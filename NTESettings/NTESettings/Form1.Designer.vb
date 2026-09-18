<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lblPath = New System.Windows.Forms.Label()
        Me.txtLogPath = New System.Windows.Forms.TextBox()
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.btnExportEncrypted = New System.Windows.Forms.Button()
        Me.btnExportPlain = New System.Windows.Forms.Button()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.grpMyData = New System.Windows.Forms.GroupBox()
        Me.lblLocationValue = New System.Windows.Forms.Label()
        Me.lblPlayerStateBaseValue = New System.Windows.Forms.Label()
        Me.lblControlledValue = New System.Windows.Forms.Label()
        Me.lblMapValue = New System.Windows.Forms.Label()
        Me.lblCharacterNameValue = New System.Windows.Forms.Label()
        Me.lblCurrentCharacterValue = New System.Windows.Forms.Label()
        Me.lblMyUidValue = New System.Windows.Forms.Label()
        Me.lblMyUserNameValue = New System.Windows.Forms.Label()
        Me.grpOtherIds = New System.Windows.Forms.GroupBox()
        Me.txtOtherIds = New System.Windows.Forms.TextBox()
        Me.grpRoleNames = New System.Windows.Forms.GroupBox()
        Me.txtRoleNames = New System.Windows.Forms.TextBox()
        Me.grpEnemies = New System.Windows.Forms.GroupBox()
        Me.txtEnemies = New System.Windows.Forms.TextBox()
        Me.txtEditor = New System.Windows.Forms.RichTextBox()
        Me.grpMyData.SuspendLayout()
        Me.grpOtherIds.SuspendLayout()
        Me.grpRoleNames.SuspendLayout()
        Me.grpEnemies.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblPath
        '
        Me.lblPath.AutoSize = True
        Me.lblPath.Location = New System.Drawing.Point(12, 15)
        Me.lblPath.Name = "lblPath"
        Me.lblPath.Size = New System.Drawing.Size(85, 13)
        Me.lblPath.TabIndex = 0
        Me.lblPath.Text = "Ruta del archivo"
        '
        'txtLogPath
        '
        Me.txtLogPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtLogPath.Location = New System.Drawing.Point(102, 12)
        Me.txtLogPath.Name = "txtLogPath"
        Me.txtLogPath.Size = New System.Drawing.Size(715, 20)
        Me.txtLogPath.TabIndex = 1
        '
        'btnBrowse
        '
        Me.btnBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnBrowse.Location = New System.Drawing.Point(823, 10)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(79, 23)
        Me.btnBrowse.TabIndex = 2
        Me.btnBrowse.Text = "Examinar"
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'btnStart
        '
        Me.btnStart.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnStart.Location = New System.Drawing.Point(908, 10)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(80, 23)
        Me.btnStart.TabIndex = 3
        Me.btnStart.Text = "Iniciar"
        Me.btnStart.UseVisualStyleBackColor = True
        '
        'btnStop
        '
        Me.btnStop.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnStop.Enabled = False
        Me.btnStop.Location = New System.Drawing.Point(908, 39)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(80, 23)
        Me.btnStop.TabIndex = 6
        Me.btnStop.Text = "Detener"
        Me.btnStop.UseVisualStyleBackColor = True
        '
        'btnExportEncrypted
        '
        Me.btnExportEncrypted.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnExportEncrypted.Location = New System.Drawing.Point(610, 39)
        Me.btnExportEncrypted.Name = "btnExportEncrypted"
        Me.btnExportEncrypted.Size = New System.Drawing.Size(143, 23)
        Me.btnExportEncrypted.TabIndex = 4
        Me.btnExportEncrypted.Text = "Exportar cifrado"
        Me.btnExportEncrypted.UseVisualStyleBackColor = True
        '
        'btnExportPlain
        '
        Me.btnExportPlain.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnExportPlain.Location = New System.Drawing.Point(759, 39)
        Me.btnExportPlain.Name = "btnExportPlain"
        Me.btnExportPlain.Size = New System.Drawing.Size(143, 23)
        Me.btnExportPlain.TabIndex = 5
        Me.btnExportPlain.Text = "Exportar sin cifrar"
        Me.btnExportPlain.UseVisualStyleBackColor = True
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(12, 44)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(32, 13)
        Me.lblStatus.TabIndex = 7
        Me.lblStatus.Text = "Listo."
        '
        'grpMyData
        '
        Me.grpMyData.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpMyData.Controls.Add(Me.lblLocationValue)
        Me.grpMyData.Controls.Add(Me.lblPlayerStateBaseValue)
        Me.grpMyData.Controls.Add(Me.lblControlledValue)
        Me.grpMyData.Controls.Add(Me.lblMapValue)
        Me.grpMyData.Controls.Add(Me.lblCharacterNameValue)
        Me.grpMyData.Controls.Add(Me.lblCurrentCharacterValue)
        Me.grpMyData.Controls.Add(Me.lblMyUidValue)
        Me.grpMyData.Controls.Add(Me.lblMyUserNameValue)
        Me.grpMyData.Location = New System.Drawing.Point(15, 68)
        Me.grpMyData.Name = "grpMyData"
        Me.grpMyData.Size = New System.Drawing.Size(973, 194)
        Me.grpMyData.TabIndex = 8
        Me.grpMyData.TabStop = False
        Me.grpMyData.Text = "Con Mi Data"
        '
        'lblLocationValue
        '
        Me.lblLocationValue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblLocationValue.AutoEllipsis = True
        Me.lblLocationValue.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLocationValue.ForeColor = System.Drawing.Color.Red
        Me.lblLocationValue.Location = New System.Drawing.Point(12, 170)
        Me.lblLocationValue.Name = "lblLocationValue"
        Me.lblLocationValue.Size = New System.Drawing.Size(945, 17)
        Me.lblLocationValue.TabIndex = 7
        Me.lblLocationValue.Text = ""
        '
        'lblPlayerStateBaseValue
        '
        Me.lblPlayerStateBaseValue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblPlayerStateBaseValue.AutoEllipsis = True
        Me.lblPlayerStateBaseValue.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPlayerStateBaseValue.ForeColor = System.Drawing.Color.LimeGreen
        Me.lblPlayerStateBaseValue.Location = New System.Drawing.Point(12, 149)
        Me.lblPlayerStateBaseValue.Name = "lblPlayerStateBaseValue"
        Me.lblPlayerStateBaseValue.Size = New System.Drawing.Size(945, 17)
        Me.lblPlayerStateBaseValue.TabIndex = 6
        Me.lblPlayerStateBaseValue.Text = ""
        '
        'lblControlledValue
        '
        Me.lblControlledValue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblControlledValue.AutoEllipsis = True
        Me.lblControlledValue.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblControlledValue.ForeColor = System.Drawing.Color.LimeGreen
        Me.lblControlledValue.Location = New System.Drawing.Point(12, 128)
        Me.lblControlledValue.Name = "lblControlledValue"
        Me.lblControlledValue.Size = New System.Drawing.Size(945, 17)
        Me.lblControlledValue.TabIndex = 5
        Me.lblControlledValue.Text = ""
        '
        'lblMapValue
        '
        Me.lblMapValue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblMapValue.AutoEllipsis = True
        Me.lblMapValue.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMapValue.ForeColor = System.Drawing.Color.LimeGreen
        Me.lblMapValue.Location = New System.Drawing.Point(12, 107)
        Me.lblMapValue.Name = "lblMapValue"
        Me.lblMapValue.Size = New System.Drawing.Size(945, 17)
        Me.lblMapValue.TabIndex = 4
        Me.lblMapValue.Text = ""
        '
        'lblCharacterNameValue
        '
        Me.lblCharacterNameValue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblCharacterNameValue.AutoEllipsis = True
        Me.lblCharacterNameValue.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCharacterNameValue.ForeColor = System.Drawing.Color.LimeGreen
        Me.lblCharacterNameValue.Location = New System.Drawing.Point(12, 86)
        Me.lblCharacterNameValue.Name = "lblCharacterNameValue"
        Me.lblCharacterNameValue.Size = New System.Drawing.Size(945, 17)
        Me.lblCharacterNameValue.TabIndex = 3
        Me.lblCharacterNameValue.Text = ""
        '
        'lblCurrentCharacterValue
        '
        Me.lblCurrentCharacterValue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblCurrentCharacterValue.AutoEllipsis = True
        Me.lblCurrentCharacterValue.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCurrentCharacterValue.ForeColor = System.Drawing.Color.LimeGreen
        Me.lblCurrentCharacterValue.Location = New System.Drawing.Point(12, 65)
        Me.lblCurrentCharacterValue.Name = "lblCurrentCharacterValue"
        Me.lblCurrentCharacterValue.Size = New System.Drawing.Size(945, 17)
        Me.lblCurrentCharacterValue.TabIndex = 2
        Me.lblCurrentCharacterValue.Text = ""
        '
        'lblMyUidValue
        '
        Me.lblMyUidValue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblMyUidValue.AutoEllipsis = True
        Me.lblMyUidValue.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMyUidValue.ForeColor = System.Drawing.Color.LimeGreen
        Me.lblMyUidValue.Location = New System.Drawing.Point(12, 44)
        Me.lblMyUidValue.Name = "lblMyUidValue"
        Me.lblMyUidValue.Size = New System.Drawing.Size(945, 17)
        Me.lblMyUidValue.TabIndex = 1
        Me.lblMyUidValue.Text = ""
        '
        'lblMyUserNameValue
        '
        Me.lblMyUserNameValue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblMyUserNameValue.AutoEllipsis = True
        Me.lblMyUserNameValue.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMyUserNameValue.ForeColor = System.Drawing.Color.LimeGreen
        Me.lblMyUserNameValue.Location = New System.Drawing.Point(12, 23)
        Me.lblMyUserNameValue.Name = "lblMyUserNameValue"
        Me.lblMyUserNameValue.Size = New System.Drawing.Size(945, 17)
        Me.lblMyUserNameValue.TabIndex = 0
        Me.lblMyUserNameValue.Text = ""
        '
        'grpOtherIds
        '
        Me.grpOtherIds.Controls.Add(Me.txtOtherIds)
        Me.grpOtherIds.Location = New System.Drawing.Point(15, 268)
        Me.grpOtherIds.Name = "grpOtherIds"
        Me.grpOtherIds.Size = New System.Drawing.Size(480, 115)
        Me.grpOtherIds.TabIndex = 9
        Me.grpOtherIds.TabStop = False
        Me.grpOtherIds.Text = "Otros"
        '
        'txtOtherIds
        '
        Me.txtOtherIds.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtOtherIds.BackColor = System.Drawing.Color.Black
        Me.txtOtherIds.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtOtherIds.ForeColor = System.Drawing.Color.LimeGreen
        Me.txtOtherIds.Location = New System.Drawing.Point(12, 20)
        Me.txtOtherIds.MaxLength = 2147483647
        Me.txtOtherIds.Multiline = True
        Me.txtOtherIds.Name = "txtOtherIds"
        Me.txtOtherIds.ReadOnly = True
        Me.txtOtherIds.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtOtherIds.Size = New System.Drawing.Size(456, 83)
        Me.txtOtherIds.TabIndex = 0
        Me.txtOtherIds.WordWrap = False
        '
        'grpRoleNames
        '
        Me.grpRoleNames.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpRoleNames.Controls.Add(Me.txtRoleNames)
        Me.grpRoleNames.Location = New System.Drawing.Point(508, 268)
        Me.grpRoleNames.Name = "grpRoleNames"
        Me.grpRoleNames.Size = New System.Drawing.Size(480, 115)
        Me.grpRoleNames.TabIndex = 10
        Me.grpRoleNames.TabStop = False
        Me.grpRoleNames.Text = "RoleNames"
        '
        'txtRoleNames
        '
        Me.txtRoleNames.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtRoleNames.BackColor = System.Drawing.Color.Black
        Me.txtRoleNames.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtRoleNames.ForeColor = System.Drawing.Color.LimeGreen
        Me.txtRoleNames.Location = New System.Drawing.Point(12, 20)
        Me.txtRoleNames.MaxLength = 2147483647
        Me.txtRoleNames.Multiline = True
        Me.txtRoleNames.Name = "txtRoleNames"
        Me.txtRoleNames.ReadOnly = True
        Me.txtRoleNames.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtRoleNames.Size = New System.Drawing.Size(456, 83)
        Me.txtRoleNames.TabIndex = 0
        Me.txtRoleNames.WordWrap = False
        '
        'grpEnemies
        '
        Me.grpEnemies.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpEnemies.Controls.Add(Me.txtEnemies)
        Me.grpEnemies.Location = New System.Drawing.Point(15, 390)
        Me.grpEnemies.Name = "grpEnemies"
        Me.grpEnemies.Size = New System.Drawing.Size(973, 130)
        Me.grpEnemies.TabIndex = 11
        Me.grpEnemies.TabStop = False
        Me.grpEnemies.Text = "Enemigos"
        '
        'txtEnemies
        '
        Me.txtEnemies.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtEnemies.BackColor = System.Drawing.Color.Black
        Me.txtEnemies.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtEnemies.ForeColor = System.Drawing.Color.LimeGreen
        Me.txtEnemies.Location = New System.Drawing.Point(12, 20)
        Me.txtEnemies.MaxLength = 2147483647
        Me.txtEnemies.Multiline = True
        Me.txtEnemies.Name = "txtEnemies"
        Me.txtEnemies.ReadOnly = True
        Me.txtEnemies.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtEnemies.Size = New System.Drawing.Size(949, 98)
        Me.txtEnemies.TabIndex = 0
        Me.txtEnemies.WordWrap = False
        '
        'txtEditor
        '
        Me.txtEditor.AcceptsTab = True
        Me.txtEditor.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtEditor.BackColor = System.Drawing.Color.Black
        Me.txtEditor.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtEditor.ForeColor = System.Drawing.Color.LimeGreen
        Me.txtEditor.Location = New System.Drawing.Point(15, 527)
        Me.txtEditor.MaxLength = 2147483647
        Me.txtEditor.Multiline = True
        Me.txtEditor.Name = "txtEditor"
        Me.txtEditor.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both
        Me.txtEditor.Size = New System.Drawing.Size(973, 221)
        Me.txtEditor.TabIndex = 12
        Me.txtEditor.WordWrap = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1000, 760)
        Me.Controls.Add(Me.txtEditor)
        Me.Controls.Add(Me.grpEnemies)
        Me.Controls.Add(Me.grpRoleNames)
        Me.Controls.Add(Me.grpOtherIds)
        Me.Controls.Add(Me.grpMyData)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.btnExportPlain)
        Me.Controls.Add(Me.btnExportEncrypted)
        Me.Controls.Add(Me.btnStop)
        Me.Controls.Add(Me.btnStart)
        Me.Controls.Add(Me.btnBrowse)
        Me.Controls.Add(Me.txtLogPath)
        Me.Controls.Add(Me.lblPath)
        Me.MinimumSize = New System.Drawing.Size(900, 620)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "HT Live Log Viewer + Decryptor"
        Me.grpMyData.ResumeLayout(False)
        Me.grpOtherIds.ResumeLayout(False)
        Me.grpOtherIds.PerformLayout()
        Me.grpRoleNames.ResumeLayout(False)
        Me.grpRoleNames.PerformLayout()
        Me.grpEnemies.ResumeLayout(False)
        Me.grpEnemies.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblPath As System.Windows.Forms.Label
    Friend WithEvents txtLogPath As System.Windows.Forms.TextBox
    Friend WithEvents btnBrowse As System.Windows.Forms.Button
    Friend WithEvents btnStart As System.Windows.Forms.Button
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents btnExportEncrypted As System.Windows.Forms.Button
    Friend WithEvents btnExportPlain As System.Windows.Forms.Button
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents grpMyData As System.Windows.Forms.GroupBox
    Friend WithEvents lblLocationValue As System.Windows.Forms.Label
    Friend WithEvents lblPlayerStateBaseValue As System.Windows.Forms.Label
    Friend WithEvents lblControlledValue As System.Windows.Forms.Label
    Friend WithEvents lblMapValue As System.Windows.Forms.Label
    Friend WithEvents lblCharacterNameValue As System.Windows.Forms.Label
    Friend WithEvents lblCurrentCharacterValue As System.Windows.Forms.Label
    Friend WithEvents lblMyUidValue As System.Windows.Forms.Label
    Friend WithEvents lblMyUserNameValue As System.Windows.Forms.Label
    Friend WithEvents grpOtherIds As System.Windows.Forms.GroupBox
    Friend WithEvents txtOtherIds As System.Windows.Forms.TextBox
    Friend WithEvents grpRoleNames As System.Windows.Forms.GroupBox
    Friend WithEvents txtRoleNames As System.Windows.Forms.TextBox
    Friend WithEvents grpEnemies As System.Windows.Forms.GroupBox
    Friend WithEvents txtEnemies As System.Windows.Forms.TextBox
    Friend WithEvents txtEditor As System.Windows.Forms.RichTextBox
End Class
