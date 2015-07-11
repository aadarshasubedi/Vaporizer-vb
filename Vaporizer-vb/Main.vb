Imports System.ComponentModel

Public Class Main

    Public filename As String
    Private goBool As Boolean = False
    Dim sizeC As New Size(212, 210), sizeO As New Size(212, 117)

    Sub Go_Click(sender As Object, e As EventArgs) Handles Go.Click
        If (goBool = False) Then

            goBool = True
            Dim ofd As New OpenFileDialog

            ofd.Title = "Choose .wav file..."
            ofd.Filter = "Wave File|*.wav"

            If (ofd.ShowDialog = DialogResult.OK) Then
                If (ofd.FileName IsNot Nothing) Then
                    fileLocation.Text = ofd.FileName.ToString
                    filename = fileLocation.Text

                    Size = sizeC
                    gif.Visible = True
                    Go.Text = "Vaporizing..."
                    Go.Enabled = False

                    Vaporizer.Program.Main()

                    gif.Visible = False
                    Size = sizeO
                    Go.Text = "Complete!"

                    Go.Enabled = True
                End If
            End If
        Else
            Application.Restart()
        End If
    End Sub
End Class
