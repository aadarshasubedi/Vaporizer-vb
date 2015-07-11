<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Main
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Go = New System.Windows.Forms.Button()
        Me.fileLocation = New System.Windows.Forms.TextBox()
        Me.gif = New System.Windows.Forms.PictureBox()
        CType(Me.gif, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Go
        '
        Me.Go.BackColor = System.Drawing.SystemColors.ButtonHighlight
        Me.Go.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Go.Location = New System.Drawing.Point(12, 38)
        Me.Go.Name = "Go"
        Me.Go.Size = New System.Drawing.Size(176, 32)
        Me.Go.TabIndex = 0
        Me.Go.Text = "Choose .wav"
        Me.Go.UseVisualStyleBackColor = False
        '
        'fileLocation
        '
        Me.fileLocation.Location = New System.Drawing.Point(12, 12)
        Me.fileLocation.Name = "fileLocation"
        Me.fileLocation.ReadOnly = True
        Me.fileLocation.Size = New System.Drawing.Size(176, 20)
        Me.fileLocation.TabIndex = 1
        Me.fileLocation.Text = "Please choose a .wav"
        '
        'gif
        '
        Me.gif.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gif.Image = Global.Vaporizer_vb.My.Resources.Resources.bg
        Me.gif.Location = New System.Drawing.Point(0, 0)
        Me.gif.Name = "gif"
        Me.gif.Size = New System.Drawing.Size(196, 78)
        Me.gif.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.gif.TabIndex = 2
        Me.gif.TabStop = False
        Me.gif.Visible = False
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.ClientSize = New System.Drawing.Size(196, 78)
        Me.Controls.Add(Me.gif)
        Me.Controls.Add(Me.fileLocation)
        Me.Controls.Add(Me.Go)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "Main"
        Me.ShowIcon = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Vaporizer"
        CType(Me.gif, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Go As Button
    Friend WithEvents fileLocation As TextBox
    Friend WithEvents gif As PictureBox
End Class
