Imports Vaporizer_vb.Mike.Rules
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace Vaporizer
    Class Program

        Shared Sub Main()
            Dim waveheader As Byte() = Nothing, wavedata As Byte() = Nothing
            Dim sampleRate As Integer = 0
            Dim in_data_l As Single() = Nothing, in_data_r As Single() = Nothing

            Dim shift = 0.6F

            Dim inFile As String = Vaporizer_vb.Main.filename

            If File.Exists(inFile) Then
                Dim wav As Byte() = File.ReadAllBytes(inFile)
                wavReader.Main.GetWaveData(inFile, waveheader, wavedata, sampleRate, in_data_l, in_data_r, wav)
                MsgBox("Processing..." & vbLf & "Please wait. This can take up to 5 minutes depending on the file size...", MsgBoxStyle.Information)

                If in_data_l IsNot Nothing Then
                    PitchShifter.PitchShift(shift, in_data_l.Length, CLng(1024), CLng(10), sampleRate, in_data_l)
                End If

                If in_data_r IsNot Nothing Then
                    PitchShifter.PitchShift(shift, in_data_r.Length, CLng(1024), CLng(10), sampleRate, in_data_r)
                End If

                Dim buffer As Byte() = New Byte(wavedata.Length - 1) {}
                Array.Copy(wavedata, buffer, wavedata.Length)

                wavReader.Main.GetWaveData(in_data_l, in_data_r, wavedata)

                Dim noChanges As Boolean = True
                For i As Integer = 0 To wavedata.Length - 1
                    If wavedata(i) <> buffer(i) Then
                        noChanges = False
                        MsgBox("Complete.", MsgBoxStyle.Information)
                        Exit For
                    End If
                Next

                If noChanges Then
                    MsgBox("ERROR: Something went wrong. Check your privileges?", MsgBoxStyle.Critical, Title:="Error.")
                End If

                Dim targetFilePath As String = inFile.Remove(inFile.Length - 4, 4) & "_vaporized.wav"

                If File.Exists(targetFilePath) Then
                    File.Delete(targetFilePath)
                End If

                Using out As New BinaryWriter(File.OpenWrite(targetFilePath))
                    out.Write(waveheader)
                    out.Write(wavedata)
                End Using
            Else
                MsgBox("File not found?", MsgBoxStyle.Critical, Title:="404")
            End If
        End Sub
    End Class
End Namespace