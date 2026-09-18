Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Threading.Tasks

Public Class Form1

    Private Const DefaultKey As String = "UVbP6pjjw5KZhvddie3tfhg1pVkkveY8"
    Private Const PollIntervalMilliseconds As Integer = 500
    Private Const MaxListItems As Integer = 4
    Private Const AppendBatchLineCount As Integer = 250
    Private Const ReadBufferSize As Integer = 32768
    Private Const CoordinateRegexPattern As String = "X=-?\d+(?:\.\d+)?\s+Y=-?\d+(?:\.\d+)?\s+Z=-?\d+(?:\.\d+)?"
    Private Shared ReadOnly Utf8NoBom As New UTF8Encoding(False)
    Private Shared ReadOnly HighlightColor As Color = Color.LimeGreen
    Private Shared ReadOnly LocationColor As Color = Color.Red
    Private Shared ReadOnly UidRegex As New Regex("\b(?:RoleID|Player)\s*\[\s*(\d+)\s*\]|\brole_id\s*=\s*(\d+)", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Private Shared ReadOnly OverlapRoleIdRegex As New Regex("\bOverlapRoleId\s*=\s*(\d+)", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Private Shared ReadOnly RoleNameRegex As New Regex("AHTPlayerState::OnRep_RoleName\s+(.+)$", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Private Shared ReadOnly CurrentCharacterRegex As New Regex("\b(?:Character|OtherActor|Class)\s*=\s*player_[^_\s]+_([^\s,\];)]+)", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Private Shared ReadOnly CurrentCharacterUidRegex As New Regex("\bRoleId\s*=\s*(\d+)|\b(?:RoleID|Player)\s*\[\s*(\d+)\s*\]|\brole_id\s*=\s*(\d+)", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Private Shared ReadOnly RoleLocationRegex As New Regex("\bLocation\s*=\s*(" & CoordinateRegexPattern & ").*?\bRoleID\s*=\s*(\d+)", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Private Shared ReadOnly PlayerLocationRegex As New Regex("\bPlayer\s*\[\s*(\d+)\s*\].*?\bPlayerLocation\s+is\s+(" & CoordinateRegexPattern & ")", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Private Shared ReadOnly RoleIdEqualsRegex As New Regex("\bRoleI[Dd]\s*=\s*(\d+)", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Private Shared ReadOnly CharacterNameParamRegex As New Regex("\bCharacterName\s*=\s*([^,\s]+)", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Private Shared ReadOnly MapParamRegex As New Regex("\bMap\s*=\s*([^,\s]+)", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Private Shared ReadOnly ControlledParamRegex As New Regex("\bbControlled\s*=\s*([01])", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Private Shared ReadOnly ParenthesizedLocationRegex As New Regex("\bLocation\s*=\s*(\([^)]+\))", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Private Shared ReadOnly ParenthesizedRotationRegex As New Regex("\bRotation\s*=\s*(\([^)]+\))", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Private Shared ReadOnly OnPostClientReadyLocationRegex As New Regex("AHTPlayerState::OnPostClientReady.*?\bRoleID\s*=\s*(\d+).*?\bCharacterName\s*=\s*([^,\s]+).*?\bLocation\s*=\s*(" & CoordinateRegexPattern & ")", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Private Shared ReadOnly PlayerStateBaseActorRegex As New Regex("\b([A-Za-z0-9_]+_C_\d+)\b", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Private Shared ReadOnly AllowedTerms As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        "mon_17_BP_Bank_DataPack0",
        "mon_27_BP_Bank_DataPack0",
        "mon_020_BP_Bank_02_DataPack0",
        "mon_028_BP_Bank_DataPack0",
        "mon_028_Patrol_BP_Bank_DataPack0",
        "mon_028_bank_Turn_90_BP_DataPack0",
        "mon_022_BP_BookShelf_Bank_DataPack0",
        "mon_029_BP_Bank_DataPack0",
        "mon_028_BP_Bank_nosummon_DataPack0",
        "mon_028_bank_Turn_90_BP_nosummon_DataPack0",
        "mon_020_BP_Bank_03_DataPack0",
        "mon_05_BP_World_OnWall_Bank_DataPack0",
        "mon_05_BP_Bank_DataPack0",
        "mon_016_BP_Bank_DataPack0",
        "Boss_017_BP_BankBoss_DataPack0",
        "mon_037_BP_Bank_DataPack0",
        "mon_023_BP_Bank_DataPack0",
        "mon_05_BP_World_OnWall_Bank_Treasure_DataPack0",
        "mon_17_BP_Bank_Treasure_DataPack0",
        "mon_020_BP_Bank_01_Treasure_DataPack0",
        "Player"
    }

    Private _watchTask As Task
    Private _watchCancellation As CancellationTokenSource
    Private _lastPosition As Long
    Private _lastReadBuffer As String
    Private ReadOnly _dataLock As New Object()
    Private ReadOnly _otherIdValues As New List(Of String)()
    Private ReadOnly _otherNameLines As New List(Of String)()
    Private ReadOnly _enemyCharacterNames As New List(Of String)()
    Private ReadOnly _enemyActorNames As New List(Of String)()
    Private ReadOnly _locationByUid As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
    Private _myUid As String = String.Empty
    Private _myUserName As String = String.Empty
    Private _currentCharacterName As String = String.Empty
    Private _currentCharacterNameValue As String = String.Empty
    Private _currentMapText As String = String.Empty
    Private _currentControlledText As String = String.Empty
    Private _currentPlayerStateBaseText As String = String.Empty
    Private _currentLocationText As String = String.Empty

    Private Class DisplayLine
        Public Sub New(ByVal text As String, ByVal foreColor As Color)
            Me.Text = text
            Me.ForeColor = foreColor
        End Sub

        Public Property Text As String
        Public Property ForeColor As Color
    End Class

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        txtLogPath.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HT\Saved_Global\Logs\HT.log")
        txtEditor.ForeColor = HighlightColor
        txtEditor.SelectionColor = HighlightColor
        ResetDetectedData()
    End Sub

    Private Sub btnBrowse_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnBrowse.Click
        Using dialog As New OpenFileDialog()
            dialog.Title = "Selecciona un archivo"
            dialog.Filter = "Archivos de texto y configuracion (*.log;*.txt;*.ini)|*.log;*.txt;*.ini|Todos los archivos (*.*)|*.*"
            dialog.FileName = txtLogPath.Text

            If dialog.ShowDialog(Me) = DialogResult.OK Then
                txtLogPath.Text = dialog.FileName
            End If
        End Using
    End Sub

    Private Sub btnStart_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnStart.Click
        If _watchTask IsNot Nothing AndAlso Not _watchTask.IsCompleted Then
            Return
        End If

        Dim filePath As String = txtLogPath.Text.Trim()
        If filePath.Length = 0 Then
            MessageBox.Show("Indica la ruta del archivo.", "Ruta requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtLogPath.Focus()
            Return
        End If

        If _watchCancellation IsNot Nothing Then
            _watchCancellation.Dispose()
        End If

        ResetEditor()
        ResetDetectedData()
        _lastPosition = 0
        _lastReadBuffer = String.Empty
        _watchCancellation = New CancellationTokenSource()

        btnStart.Enabled = False
        btnStop.Enabled = True
        txtLogPath.Enabled = False
        btnBrowse.Enabled = False
        SetStatus("Leyendo: " & filePath)

        _watchTask = WatchFileAsync(filePath, _watchCancellation.Token)
    End Sub

    Private Async Sub btnStop_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnStop.Click
        Await StopWatchingAsync()
    End Sub

    Private Sub btnExportEncrypted_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnExportEncrypted.Click
        ExportCurrentView(True)
    End Sub

    Private Sub btnExportPlain_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnExportPlain.Click
        ExportCurrentView(False)
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
        If _watchCancellation IsNot Nothing Then
            _watchCancellation.Cancel()
        End If
    End Sub

    Private Async Function StopWatchingAsync() As Task
        Dim cancellation As CancellationTokenSource = _watchCancellation
        Dim runningTask As Task = _watchTask

        btnStop.Enabled = False

        If cancellation IsNot Nothing Then
            cancellation.Cancel()
        End If

        If runningTask IsNot Nothing Then
            Try
                Await runningTask
            Catch ex As OperationCanceledException
            End Try
        End If

        If cancellation IsNot Nothing Then
            cancellation.Dispose()
        End If

        If ReferenceEquals(_watchCancellation, cancellation) Then
            _watchCancellation = Nothing
        End If

        If ReferenceEquals(_watchTask, runningTask) Then
            _watchTask = Nothing
        End If

        RestoreIdleState()
        SetStatus("Detenido.")
    End Function

    Private Async Function WatchFileAsync(ByVal filePath As String, ByVal cancellationToken As CancellationToken) As Task
        Dim lastMissingMessageShown As Boolean = False

        Try
            While Not cancellationToken.IsCancellationRequested
                Try
                    If Not File.Exists(filePath) Then
                        If Not lastMissingMessageShown Then
                            SetStatus("Esperando a que exista el archivo...")
                            lastMissingMessageShown = True
                        End If
                    Else
                        lastMissingMessageShown = False

                        If _lastPosition = 0 Then
                            Await LoadAllContentAsync(filePath, cancellationToken).ConfigureAwait(False)
                        Else
                            Await ReadNewContentAsync(filePath, cancellationToken).ConfigureAwait(False)
                        End If
                    End If
                Catch ex As OperationCanceledException
                    Throw
                Catch ex As IOException
                    SetStatus("Archivo ocupado, reintentando lectura: " & ex.Message)
                Catch ex As UnauthorizedAccessException
                    SetStatus("Sin permiso temporal para leer, reintentando: " & ex.Message)
                End Try

                Await Task.Delay(PollIntervalMilliseconds, cancellationToken).ConfigureAwait(False)
            End While
        Catch ex As OperationCanceledException
        Catch ex As Exception
            SetStatus("Ocurrio un error durante la lectura: " & ex.Message)
        Finally
            RestoreIdleStateOnUi()
        End Try
    End Function

    Private Sub RestoreIdleStateOnUi()
        If Me.IsDisposed OrElse Not Me.IsHandleCreated Then
            Return
        End If

        If Me.InvokeRequired Then
            Try
                Me.BeginInvoke(New MethodInvoker(AddressOf RestoreIdleState))
            Catch ex As InvalidOperationException
            End Try
        Else
            RestoreIdleState()
        End If
    End Sub

    Private Sub RestoreIdleState()
        btnStart.Enabled = True
        btnStop.Enabled = False
        txtLogPath.Enabled = True
        btnBrowse.Enabled = True
    End Sub

    Private Async Function LoadAllContentAsync(ByVal filePath As String, ByVal cancellationToken As CancellationToken) As Task
        Dim pendingLines As New List(Of DisplayLine)(AppendBatchLineCount)

        Using stream As FileStream = OpenLogStream(filePath)
            Using reader As New StreamReader(stream, Encoding.UTF8, True)
                While Not reader.EndOfStream
                    cancellationToken.ThrowIfCancellationRequested()

                    Dim line As String = Await reader.ReadLineAsync().ConfigureAwait(False)
                    If line IsNot Nothing Then
                        AppendProcessedLine(line, pendingLines)
                    End If

                    If pendingLines.Count >= AppendBatchLineCount Then
                        FlushPendingLines(pendingLines)
                    End If
                End While

                _lastPosition = stream.Position
            End Using
        End Using

        FlushPendingLines(pendingLines)
        SetStatus("Mostrando coincidencias en verde y esperando nuevas lineas...")
    End Function

    Private Async Function ReadNewContentAsync(ByVal filePath As String, ByVal cancellationToken As CancellationToken) As Task
        Dim pendingLines As New List(Of DisplayLine)(AppendBatchLineCount)

        Using stream As FileStream = OpenLogStream(filePath)
            If stream.Length < _lastPosition Then
                ResetEditorOnUi()
                _lastPosition = 0
                _lastReadBuffer = String.Empty
                Return
            End If

            If stream.Length = _lastPosition Then
                Return
            End If

            stream.Seek(_lastPosition, SeekOrigin.Begin)

            Using reader As New StreamReader(stream, Encoding.UTF8, True)
                Dim buffer(ReadBufferSize - 1) As Char
                Dim readCount As Integer

                Do
                    cancellationToken.ThrowIfCancellationRequested()

                    readCount = Await reader.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(False)
                    If readCount > 0 Then
                        ProcessChunk(New String(buffer, 0, readCount), pendingLines)
                    End If

                    If pendingLines.Count >= AppendBatchLineCount Then
                        FlushPendingLines(pendingLines)
                    End If
                Loop While readCount > 0

                _lastPosition = stream.Position
            End Using
        End Using

        FlushPendingLines(pendingLines)
    End Function

    Private Function OpenLogStream(ByVal filePath As String) As FileStream
        Return New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite Or FileShare.Delete, ReadBufferSize, FileOptions.Asynchronous Or FileOptions.SequentialScan)
    End Function

    Private Sub ProcessChunk(ByVal chunk As String, ByVal pendingLines As List(Of DisplayLine))
        Dim normalizedChunk As String = _lastReadBuffer & chunk
        normalizedChunk = normalizedChunk.Replace(vbCrLf, vbLf)
        normalizedChunk = normalizedChunk.Replace(vbCr, vbLf)

        Dim lines() As String = normalizedChunk.Split(New Char() {ControlChars.Lf})
        Dim lastIndex As Integer = lines.Length - 1
        Dim endedWithLineBreak As Boolean = normalizedChunk.EndsWith(vbLf)
        Dim index As Integer

        _lastReadBuffer = String.Empty

        For index = 0 To lastIndex
            If index = lastIndex AndAlso Not endedWithLineBreak Then
                _lastReadBuffer = lines(index)
            Else
                AppendProcessedLine(lines(index), pendingLines)
            End If
        Next
    End Sub

    Private Sub AppendProcessedLine(ByVal originalLine As String, ByVal pendingLines As List(Of DisplayLine))
        Dim wasEncrypted As Boolean = False
        Dim displayLine As String = DecryptLine(originalLine, wasEncrypted)
        Dim visibleLines() As String = BuildVisibleLines(displayLine)
        Dim index As Integer

        For index = 0 To visibleLines.Length - 1
            Dim visibleLine As String = visibleLines(index)

            If IsSuppressedLine(visibleLine) Then
                Continue For
            End If

            Dim isCurrentLocationLine As Boolean = AnalyzeDataLine(visibleLine)

            If isCurrentLocationLine Then
                pendingLines.Add(New DisplayLine(visibleLine, LocationColor))
            ElseIf IsAllowedLine(visibleLine) Then
                pendingLines.Add(New DisplayLine(visibleLine, HighlightColor))
            End If
        Next
    End Sub

    Private Sub FlushPendingLines(ByVal pendingLines As List(Of DisplayLine))
        If pendingLines.Count = 0 Then
            Return
        End If

        Dim lines() As DisplayLine = pendingLines.ToArray()
        pendingLines.Clear()
        RegisterLines(lines)
    End Sub

    Private Sub RegisterLines(ByVal lines() As DisplayLine)
        If lines Is Nothing OrElse lines.Length = 0 Then
            Return
        End If

        If txtEditor.IsDisposed OrElse Not txtEditor.IsHandleCreated Then
            Return
        End If

        If txtEditor.InvokeRequired Then
            Try
                txtEditor.Invoke(New RegisterLinesDelegate(AddressOf RegisterLines), New Object() {lines})
            Catch ex As InvalidOperationException
            End Try
            Return
        End If

        txtEditor.SuspendLayout()

        Try
            Dim index As Integer

            For index = 0 To lines.Length - 1
                If txtEditor.TextLength > 0 Then
                    txtEditor.AppendText(Environment.NewLine)
                End If

                txtEditor.SelectionStart = txtEditor.TextLength
                txtEditor.SelectionLength = 0
                txtEditor.SelectionColor = lines(index).ForeColor
                txtEditor.AppendText(lines(index).Text)
            Next

            txtEditor.SelectionStart = txtEditor.TextLength
            txtEditor.SelectionLength = 0
            txtEditor.SelectionColor = HighlightColor
            txtEditor.ScrollToCaret()
        Finally
            txtEditor.ResumeLayout()
        End Try
    End Sub

    Private Sub ResetEditor()
        txtEditor.Clear()
        txtEditor.SelectionColor = HighlightColor
    End Sub

    Private Sub ResetEditorOnUi()
        If txtEditor.IsDisposed OrElse Not txtEditor.IsHandleCreated Then
            Return
        End If

        If txtEditor.InvokeRequired Then
            Try
                txtEditor.Invoke(New MethodInvoker(AddressOf ResetEditor))
            Catch ex As InvalidOperationException
            End Try
        Else
            ResetEditor()
        End If
    End Sub

    Private Sub ResetDetectedData()
        SyncLock _dataLock
            _myUid = String.Empty
            _myUserName = String.Empty
            _currentCharacterName = String.Empty
            _currentCharacterNameValue = String.Empty
            _currentMapText = String.Empty
            _currentControlledText = String.Empty
            _currentPlayerStateBaseText = String.Empty
            _currentLocationText = String.Empty
            _otherIdValues.Clear()
            _otherNameLines.Clear()
            _enemyCharacterNames.Clear()
            _enemyActorNames.Clear()
            _locationByUid.Clear()
        End SyncLock

        UpdateDetectedDataView()
    End Sub

    Private Sub UpdateDetectedDataView()
        If Me.IsDisposed OrElse Not Me.IsHandleCreated Then
            Return
        End If

        If Me.InvokeRequired Then
            Try
                Me.BeginInvoke(New MethodInvoker(AddressOf UpdateDetectedDataView))
            Catch ex As InvalidOperationException
            End Try
            Return
        End If

        Dim myUserNameText As String = String.Empty
        Dim myUidText As String = String.Empty
        Dim currentCharacterText As String = String.Empty
        Dim characterNameText As String = String.Empty
        Dim mapText As String = String.Empty
        Dim controlledText As String = String.Empty
        Dim playerStateBaseText As String = String.Empty
        Dim currentLocationText As String = String.Empty
        Dim otherIdsText As String = String.Empty
        Dim otherNamesText As String = String.Empty
        Dim enemiesText As String = String.Empty

        SyncLock _dataLock
            myUserNameText = _myUserName
            If _myUid.Length > 0 Then
                myUidText = "UID Actual: RoleID=" & _myUid
            End If

            If _currentCharacterName.Length > 0 Then
                currentCharacterText = "Personaje Actual: " & _currentCharacterName
            End If

            characterNameText = _currentCharacterNameValue
            mapText = _currentMapText
            controlledText = _currentControlledText
            playerStateBaseText = _currentPlayerStateBaseText

            If _currentLocationText.Length > 0 Then
                currentLocationText = "Location: " & _currentLocationText
            End If

            Dim otherIdLines As New List(Of String)(_otherIdValues.Count)
            For Each overlapId As String In _otherIdValues
                If IsOtherUidValue(overlapId) Then
                    otherIdLines.Add(overlapId)
                End If
            Next

            otherIdsText = String.Join(Environment.NewLine, otherIdLines.ToArray())
            otherNamesText = String.Join(Environment.NewLine, _otherNameLines.ToArray())
            enemiesText = BuildEnemyRows()
        End SyncLock

        If lblMyUserNameValue.Text <> myUserNameText Then
            lblMyUserNameValue.Text = myUserNameText
        End If

        If lblMyUidValue.Text <> myUidText Then
            lblMyUidValue.Text = myUidText
        End If

        If lblCurrentCharacterValue.Text <> currentCharacterText Then
            lblCurrentCharacterValue.Text = currentCharacterText
        End If

        If lblCharacterNameValue.Text <> characterNameText Then
            lblCharacterNameValue.Text = characterNameText
        End If

        If lblMapValue.Text <> mapText Then
            lblMapValue.Text = mapText
        End If

        If lblControlledValue.Text <> controlledText Then
            lblControlledValue.Text = controlledText
        End If

        If lblPlayerStateBaseValue.Text <> playerStateBaseText Then
            lblPlayerStateBaseValue.Text = playerStateBaseText
        End If

        If lblLocationValue.Text <> currentLocationText Then
            lblLocationValue.Text = currentLocationText
        End If

        If txtOtherIds.Text <> otherIdsText Then
            txtOtherIds.Text = otherIdsText
        End If

        If txtRoleNames.Text <> otherNamesText Then
            txtRoleNames.Text = otherNamesText
        End If

        If txtEnemies.Text <> enemiesText Then
            txtEnemies.Text = enemiesText
        End If
    End Sub

    Private Function IsAllowedLine(ByVal lineText As String) As Boolean
        If lineText Is Nothing OrElse lineText.Length = 0 Then
            Return False
        End If

        For Each allowedTerm As String In AllowedTerms
            If lineText.IndexOf(allowedTerm, StringComparison.OrdinalIgnoreCase) >= 0 Then
                Return True
            End If
        Next

        Return False
    End Function

    Private Function IsSuppressedLine(ByVal lineText As String) As Boolean
        If lineText Is Nothing OrElse lineText.Length = 0 Then
            Return True
        End If

        Return lineText.IndexOf("AHTPlayerController::Input_Move Failed! IsDisableInput", StringComparison.OrdinalIgnoreCase) >= 0 _
            OrElse lineText.IndexOf("AHTPlayerController::AbilityInput_Pressed Failed! IsDisableInput", StringComparison.OrdinalIgnoreCase) >= 0 _
            OrElse lineText.IndexOf("CreateSavedMove: Hit limit of 96 saved moves", StringComparison.OrdinalIgnoreCase) >= 0 _
            OrElse lineText.IndexOf("LogMediaUtils:", StringComparison.OrdinalIgnoreCase) >= 0
    End Function

    Private Function IsOtherUidValue(ByVal uidValue As String) As Boolean
        If uidValue Is Nothing Then
            Return False
        End If

        Dim normalizedUid As String = uidValue.Trim()
        If normalizedUid.Length = 0 OrElse normalizedUid = "0" Then
            Return False
        End If

        Return _myUid.Length = 0 OrElse Not String.Equals(normalizedUid, _myUid, StringComparison.OrdinalIgnoreCase)
    End Function

    Private Sub AddRollingValue(ByVal values As List(Of String), ByVal value As String)
        If value Is Nothing Then
            Return
        End If

        Dim cleanValue As String = value.Trim()
        If cleanValue.Length = 0 Then
            Return
        End If

        values.Add(cleanValue)

        While values.Count > MaxListItems
            values.RemoveAt(0)
        End While
    End Sub

    Private Sub RemoveOtherIdValue(ByVal uidValue As String)
        Dim index As Integer = _otherIdValues.Count - 1

        While index >= 0
            If String.Equals(_otherIdValues(index), uidValue, StringComparison.OrdinalIgnoreCase) Then
                _otherIdValues.RemoveAt(index)
            End If

            index -= 1
        End While
    End Sub

    Private Function RemoveRollingValues(ByVal values As List(Of String), ByVal value As String) As Boolean
        Dim removed As Boolean = False
        Dim index As Integer = values.Count - 1

        While index >= 0
            If String.Equals(values(index), value, StringComparison.OrdinalIgnoreCase) Then
                values.RemoveAt(index)
                removed = True
            End If

            index -= 1
        End While

        Return removed
    End Function

    Private Function BuildEnemyRows() As String
        Dim maxRows As Integer = Math.Max(_enemyCharacterNames.Count, Math.Max(_enemyActorNames.Count, _otherNameLines.Count))
        Dim rows As New List(Of String)(maxRows)
        Dim index As Integer

        For index = 0 To maxRows - 1
            Dim characterName As String = GetListValue(_enemyCharacterNames, index)
            Dim actorName As String = GetListValue(_enemyActorNames, index)
            Dim roleName As String = GetListValue(_otherNameLines, index)
            Dim row As String = String.Empty

            If characterName.Length > 0 Then
                row = "CharacterName=" & characterName
            End If

            If actorName.Length > 0 Then
                If row.Length > 0 Then
                    row &= " | "
                End If

                row &= actorName
            End If

            If roleName.Length > 0 Then
                If row.Length > 0 Then
                    row &= " - "
                End If

                row &= roleName
            End If

            If row.Length > 0 Then
                rows.Add(row)
            End If
        Next

        Return String.Join(Environment.NewLine, rows.ToArray())
    End Function

    Private Function GetListValue(ByVal values As List(Of String), ByVal index As Integer) As String
        If index < 0 OrElse index >= values.Count Then
            Return String.Empty
        End If

        Return values(index)
    End Function

    Private Function AnalyzeDataLine(ByVal lineText As String) As Boolean
        Dim isCurrentLine As Boolean = TrackPlayerCharacterBeginPlay(lineText)
        isCurrentLine = TrackOnPostClientReady(lineText) OrElse isCurrentLine
        isCurrentLine = TrackCurrentLocation(lineText) OrElse isCurrentLine

        TrackPlayerStateBaseActor(lineText)
        TrackCurrentCharacter(lineText)

        If Not ContainsLocationData(lineText) AndAlso Not IsControlledCharacterLine(lineText) Then
            TrackMyUid(lineText)
        End If

        TrackOverlapRoleIds(lineText)
        TrackRoleName(lineText)
        Return isCurrentLine
    End Function

    Private Function TrackPlayerCharacterBeginPlay(ByVal lineText As String) As Boolean
        If lineText.IndexOf("HTPlayerCharacter::BeginPlay", StringComparison.OrdinalIgnoreCase) < 0 Then
            Return False
        End If

        Dim controlled As String = FirstCapturedValue(ControlledParamRegex.Match(lineText))
        Dim roleId As String = FirstCapturedValue(RoleIdEqualsRegex.Match(lineText))
        Dim characterName As String = FirstCapturedValue(CharacterNameParamRegex.Match(lineText))
        Dim mapName As String = FirstCapturedValue(MapParamRegex.Match(lineText))
        Dim locationValue As String = FirstCapturedValue(ParenthesizedLocationRegex.Match(lineText))
        Dim rotationValue As String = FirstCapturedValue(ParenthesizedRotationRegex.Match(lineText))

        If controlled = "0" Then
            If roleId.Length > 0 AndAlso roleId <> "0" AndAlso characterName.Length > 0 Then
                SyncLock _dataLock
                    AddRollingValue(_enemyCharacterNames, characterName)
                End SyncLock
                UpdateDetectedDataView()
            End If

            Return False
        End If

        If controlled <> "1" Then
            Return False
        End If

        Dim changed As Boolean = False

        SyncLock _dataLock
            If roleId.Length > 0 AndAlso roleId <> "0" AndAlso Not String.Equals(_myUid, roleId, StringComparison.OrdinalIgnoreCase) Then
                _myUid = roleId
                changed = True
            End If

            If SyncCurrentLocationFromKnownUid() Then
                changed = True
            End If

            If RemoveRollingValues(_otherIdValues, _myUid) Then
                changed = True
            End If

            If characterName.Length > 0 Then
                Dim characterNameText As String = "CharacterName=" & characterName
                If _currentCharacterNameValue <> characterNameText Then
                    _currentCharacterNameValue = characterNameText
                    changed = True
                End If
            End If

            If mapName.Length > 0 Then
                Dim mapText As String = "Map=" & mapName
                If _currentMapText <> mapText Then
                    _currentMapText = mapText
                    changed = True
                End If
            End If

            If _currentControlledText <> "bControlled=1" Then
                _currentControlledText = "bControlled=1"
                changed = True
            End If

            Dim locationText As String = BuildLocationRotationText(locationValue, rotationValue)
            If locationText.Length > 0 AndAlso _currentLocationText <> locationText Then
                _currentLocationText = locationText
                changed = True
            End If
        End SyncLock

        If changed Then
            UpdateDetectedDataView()
        End If

        Return True
    End Function

    Private Function TrackOnPostClientReady(ByVal lineText As String) As Boolean
        Dim match As Match = OnPostClientReadyLocationRegex.Match(lineText)
        If Not match.Success Then
            Return False
        End If

        Dim roleId As String = match.Groups(1).Value.Trim()
        Dim characterName As String = match.Groups(2).Value.Trim()
        Dim locationText As String = "Location=" & match.Groups(3).Value.Trim()
        Dim changed As Boolean = False

        SyncLock _dataLock
            If _myUid.Length = 0 OrElse Not String.Equals(roleId, _myUid, StringComparison.OrdinalIgnoreCase) Then
                Return False
            End If

            If characterName.Length > 0 Then
                Dim characterNameText As String = "CharacterName=" & characterName
                If _currentCharacterNameValue <> characterNameText Then
                    _currentCharacterNameValue = characterNameText
                    changed = True
                End If
            End If

            If _currentLocationText <> locationText Then
                _currentLocationText = locationText
                changed = True
            End If
        End SyncLock

        If changed Then
            UpdateDetectedDataView()
        End If

        Return True
    End Function

    Private Sub TrackPlayerStateBaseActor(ByVal lineText As String)
        Dim match As Match = PlayerStateBaseActorRegex.Match(lineText)
        If Not match.Success Then
            Return
        End If

        Dim actorName As String = match.Groups(1).Value.Trim()
        If actorName.Length = 0 Then
            Return
        End If

        Dim changed As Boolean = False

        SyncLock _dataLock
            If lineText.IndexOf("ServerCancelEntryMatchClone", StringComparison.OrdinalIgnoreCase) >= 0 Then
                If _currentPlayerStateBaseText <> actorName Then
                    _currentPlayerStateBaseText = actorName
                    changed = True
                End If
            ElseIf lineText.IndexOf("No owning connection for actor", StringComparison.OrdinalIgnoreCase) >= 0 Then
                If _currentPlayerStateBaseText.Length = 0 OrElse Not String.Equals(actorName, _currentPlayerStateBaseText, StringComparison.OrdinalIgnoreCase) Then
                    AddRollingValue(_enemyActorNames, actorName)
                    changed = True
                End If
            End If
        End SyncLock

        If changed Then
            UpdateDetectedDataView()
        End If
    End Sub

    Private Function TrackCurrentLocation(ByVal lineText As String) As Boolean
        Dim roleLocationMatch As Match = RoleLocationRegex.Match(lineText)
        If roleLocationMatch.Success Then
            Return TrySetCurrentLocation(roleLocationMatch.Groups(2).Value, "Location=" & roleLocationMatch.Groups(1).Value)
        End If

        Dim playerLocationMatch As Match = PlayerLocationRegex.Match(lineText)
        If playerLocationMatch.Success Then
            Return TrySetCurrentLocation(playerLocationMatch.Groups(1).Value, "Location=" & playerLocationMatch.Groups(2).Value)
        End If

        Return False
    End Function

    Private Function TrySetCurrentLocation(ByVal uidValue As String, ByVal locationText As String) As Boolean
        If uidValue Is Nothing OrElse locationText Is Nothing Then
            Return False
        End If

        Dim normalizedUid As String = uidValue.Trim()
        Dim normalizedLocation As String = locationText.Trim()
        If normalizedUid.Length = 0 OrElse normalizedLocation.Length = 0 Then
            Return False
        End If

        Dim changed As Boolean = False

        SyncLock _dataLock
            _locationByUid(normalizedUid) = normalizedLocation

            If _myUid.Length = 0 OrElse Not String.Equals(normalizedUid, _myUid, StringComparison.OrdinalIgnoreCase) Then
                Return False
            End If

            If Not String.Equals(_currentLocationText, normalizedLocation, StringComparison.OrdinalIgnoreCase) Then
                _currentLocationText = normalizedLocation
                changed = True
            End If
        End SyncLock

        If changed Then
            UpdateDetectedDataView()
        End If

        Return True
    End Function

    Private Function SyncCurrentLocationFromKnownUid() As Boolean
        If _myUid.Length = 0 OrElse Not _locationByUid.ContainsKey(_myUid) Then
            Return False
        End If

        Dim knownLocation As String = _locationByUid(_myUid)
        If String.Equals(_currentLocationText, knownLocation, StringComparison.OrdinalIgnoreCase) Then
            Return False
        End If

        _currentLocationText = knownLocation
        Return True
    End Function

    Private Function ContainsLocationData(ByVal lineText As String) As Boolean
        If lineText Is Nothing Then
            Return False
        End If

        Return lineText.IndexOf("Location", StringComparison.OrdinalIgnoreCase) >= 0
    End Function

    Private Function IsControlledCharacterLine(ByVal lineText As String) As Boolean
        If lineText Is Nothing Then
            Return False
        End If

        Return lineText.IndexOf("HTPlayerCharacter::BeginPlay", StringComparison.OrdinalIgnoreCase) >= 0 _
            AndAlso ControlledParamRegex.IsMatch(lineText)
    End Function

    Private Function BuildLocationRotationText(ByVal locationValue As String, ByVal rotationValue As String) As String
        Dim parts As New List(Of String)()

        If locationValue IsNot Nothing AndAlso locationValue.Trim().Length > 0 Then
            parts.Add("Location=" & locationValue.Trim())
        End If

        If rotationValue IsNot Nothing AndAlso rotationValue.Trim().Length > 0 Then
            parts.Add("Rotation=" & rotationValue.Trim())
        End If

        Return String.Join(" ", parts.ToArray())
    End Function

    Private Sub TrackCurrentCharacter(ByVal lineText As String)
        Dim match As Match = CurrentCharacterRegex.Match(lineText)
        If Not match.Success Then
            Return
        End If

        Dim roleId As String = FirstCapturedValue(CurrentCharacterUidRegex.Match(lineText))
        Dim characterName As String = ToDisplayName(match.Groups(1).Value)
        Dim isControlled As Boolean = lineText.IndexOf("bControlled=1", StringComparison.OrdinalIgnoreCase) >= 0

        If roleId.Length = 0 AndAlso isControlled Then
            roleId = _myUid
        End If

        If characterName.Length = 0 OrElse (roleId.Length = 0 AndAlso Not isControlled) Then
            Return
        End If

        Dim changed As Boolean = False

        SyncLock _dataLock
            If roleId.Length > 0 AndAlso _myUid.Length = 0 Then
                _myUid = roleId
                changed = True
            End If

            If roleId.Length > 0 AndAlso Not String.Equals(roleId, _myUid, StringComparison.OrdinalIgnoreCase) Then
                Return
            End If

            If SyncCurrentLocationFromKnownUid() Then
                changed = True
            End If

            If RemoveRollingValues(_otherIdValues, _myUid) Then
                changed = True
            End If

            If Not String.Equals(_currentCharacterName, characterName, StringComparison.OrdinalIgnoreCase) Then
                _currentCharacterName = characterName
                changed = True
            End If
        End SyncLock

        If changed Then
            UpdateDetectedDataView()
        End If
    End Sub

    Private Sub TrackMyUid(ByVal lineText As String)
        Dim detectedUid As String = FirstCapturedValue(UidRegex.Match(lineText))
        If detectedUid.Length = 0 Then
            Return
        End If

        Dim changed As Boolean = False

        SyncLock _dataLock
            If _myUid.Length = 0 Then
                _myUid = detectedUid
                changed = True
            End If

            If SyncCurrentLocationFromKnownUid() Then
                changed = True
            End If

            If RemoveRollingValues(_otherIdValues, _myUid) Then
                changed = True
            End If
        End SyncLock

        If changed Then
            UpdateDetectedDataView()
        End If
    End Sub

    Private Sub TrackOverlapRoleIds(ByVal lineText As String)
        Dim matches As MatchCollection = OverlapRoleIdRegex.Matches(lineText)
        If matches.Count = 0 Then
            Return
        End If

        Dim changed As Boolean = False

        SyncLock _dataLock
            For Each match As Match In matches
                Dim overlapId As String = match.Groups(1).Value.Trim()
                If Not IsOtherUidValue(overlapId) Then
                    Continue For
                End If

                AddRollingValue(_otherIdValues, overlapId)
                changed = True
            Next
        End SyncLock

        If changed Then
            UpdateDetectedDataView()
        End If
    End Sub

    Private Sub TrackRoleName(ByVal lineText As String)
        Dim match As Match = RoleNameRegex.Match(lineText)
        If Not match.Success Then
            Return
        End If

        Dim roleName As String = match.Groups(1).Value.Trim()
        If roleName.Length = 0 Then
            Return
        End If

        Dim changed As Boolean = False

        SyncLock _dataLock
            If _myUserName.Length = 0 Then
                _myUserName = roleName
                changed = True
            ElseIf Not String.Equals(roleName, _myUserName, StringComparison.OrdinalIgnoreCase) Then
                AddRollingValue(_otherNameLines, roleName)
                changed = True
            End If
        End SyncLock

        If changed Then
            UpdateDetectedDataView()
        End If
    End Sub

    Private Function FirstCapturedValue(ByVal match As Match) As String
        If match Is Nothing OrElse Not match.Success Then
            Return String.Empty
        End If

        Dim index As Integer
        For index = 1 To match.Groups.Count - 1
            If match.Groups(index).Success Then
                Return match.Groups(index).Value
            End If
        Next

        Return String.Empty
    End Function

    Private Function ToDisplayName(ByVal rawName As String) As String
        If rawName Is Nothing Then
            Return String.Empty
        End If

        Dim cleanName As String = rawName.Trim().Replace("_", " ")
        If cleanName.Length = 0 Then
            Return String.Empty
        End If

        Return Char.ToUpperInvariant(cleanName.Chars(0)) & cleanName.Substring(1)
    End Function

    Private Function DecryptLine(ByVal line As String, ByRef wasEncrypted As Boolean) As String
        If line Is Nothing Then
            Return String.Empty
        End If

        If line.Length < 30 OrElse Not IsBase64Candidate(line) Then
            Return line
        End If

        Try
            Dim keyBytes() As Byte = Encoding.UTF8.GetBytes(DefaultKey)
            Dim cipherBytes() As Byte = Convert.FromBase64String(line.Trim())

            Using aes As New RijndaelManaged()
                aes.BlockSize = 128
                aes.KeySize = 256
                aes.Mode = CipherMode.ECB
                aes.Padding = PaddingMode.PKCS7
                aes.Key = keyBytes

                Using decryptor As ICryptoTransform = aes.CreateDecryptor()
                    Dim decryptedBytes() As Byte = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length)
                    Dim text As String = Encoding.UTF8.GetString(decryptedBytes).Trim()
                    wasEncrypted = True
                    Return text
                End Using
            End Using
        Catch
            Return line
        End Try
    End Function

    Private Function EncryptLine(ByVal line As String) As String
        If line Is Nothing Then
            line = String.Empty
        End If

        Dim keyBytes() As Byte = Encoding.UTF8.GetBytes(DefaultKey)
        Dim plainBytes() As Byte = Encoding.UTF8.GetBytes(line)

        Using aes As New RijndaelManaged()
            aes.BlockSize = 128
            aes.KeySize = 256
            aes.Mode = CipherMode.ECB
            aes.Padding = PaddingMode.PKCS7
            aes.Key = keyBytes

            Using encryptor As ICryptoTransform = aes.CreateEncryptor()
                Dim encryptedBytes() As Byte = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length)
                Return Convert.ToBase64String(encryptedBytes)
            End Using
        End Using
    End Function

    Private Function IsBase64Candidate(ByVal value As String) As Boolean
        Dim trimmedValue As String = value.Trim()
        If trimmedValue.Length = 0 OrElse trimmedValue.Length Mod 4 <> 0 Then
            Return False
        End If

        Dim index As Integer
        For index = 0 To trimmedValue.Length - 1
            Dim currentChar As Char = trimmedValue.Chars(index)
            Dim isValid As Boolean = Char.IsLetterOrDigit(currentChar) OrElse currentChar = "+"c OrElse currentChar = "/"c OrElse currentChar = "="c
            If Not isValid Then
                Return False
            End If
        Next

        Return True
    End Function

    Private Function BuildVisibleLines(ByVal text As String) As String()
        Dim normalizedText As String = text.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf)
        Dim rawParts() As String = normalizedText.Split(New String() {"|SPLIT|"}, StringSplitOptions.None)
        Dim visibleParts As New List(Of String)()
        Dim index As Integer

        For index = 0 To rawParts.Length - 1
            Dim part As String = rawParts(index).Trim()
            If part.Length > 0 Then
                visibleParts.Add(part)
            End If
        Next

        If visibleParts.Count = 0 Then
            Return New String() {String.Empty}
        End If

        Dim result(visibleParts.Count - 1) As String
        For index = 0 To visibleParts.Count - 1
            result(index) = visibleParts(index)
        Next

        Return result
    End Function

    Private Sub ExportCurrentView(ByVal encrypt As Boolean)
        Using dialog As New SaveFileDialog()
            dialog.Title = If(encrypt, "Exportar contenido cifrado", "Exportar contenido sin cifrar")
            dialog.Filter = "Archivos de texto (*.txt)|*.txt|Archivos INI (*.ini)|*.ini|Archivos LOG (*.log)|*.log|Todos los archivos (*.*)|*.*"
            dialog.FileName = BuildDefaultExportFileName(encrypt)

            If dialog.ShowDialog(Me) <> DialogResult.OK Then
                Return
            End If

            Dim content As String = If(encrypt, BuildEncryptedExport(), txtEditor.Text)
            File.WriteAllText(dialog.FileName, content, Utf8NoBom)
            SetStatus("Archivo exportado: " & dialog.FileName)
        End Using
    End Sub

    Private Function BuildEncryptedExport() As String
        Dim currentText As String = txtEditor.Text.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf)
        Dim lines() As String = currentText.Split(New Char() {ControlChars.Lf}, StringSplitOptions.None)
        Dim builder As New StringBuilder()
        Dim index As Integer
        Dim hasWrittenLine As Boolean = False

        For index = 0 To lines.Length - 1
            Dim cleanLine As String = lines(index).Trim()
            If cleanLine.Length = 0 Then
                Continue For
            End If

            If hasWrittenLine Then
                builder.AppendLine()
            End If

            builder.Append(EncryptLine(cleanLine & "|SPLIT|"))
            hasWrittenLine = True
        Next

        Return builder.ToString()
    End Function

    Private Function BuildDefaultExportFileName(ByVal encrypt As Boolean) As String
        Dim sourcePath As String = txtLogPath.Text.Trim()
        Dim baseName As String

        If sourcePath.Length = 0 Then
            baseName = "export"
        Else
            baseName = Path.GetFileNameWithoutExtension(sourcePath)
            If baseName.Length = 0 Then
                baseName = "export"
            End If
        End If

        If encrypt Then
            Return baseName & "_Encrypt.txt"
        End If

        Return baseName & "_Decrypt.txt"
    End Function

    Private Delegate Sub SetStatusDelegate(ByVal text As String)
    Private Delegate Sub RegisterLinesDelegate(ByVal lines() As DisplayLine)

    Private Sub SetStatus(ByVal text As String)
        If lblStatus.IsDisposed OrElse Not lblStatus.IsHandleCreated Then
            Return
        End If

        If lblStatus.InvokeRequired Then
            Try
                lblStatus.BeginInvoke(New SetStatusDelegate(AddressOf SetStatus), New Object() {text})
            Catch ex As InvalidOperationException
            End Try
            Return
        End If

        lblStatus.Text = text
    End Sub

End Class
