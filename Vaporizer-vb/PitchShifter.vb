'***************************************************************************
'* *******************CONVERTED TO VB.NET BY NANIDAYO***********************
'* NAME: PitchShift.cs
'* VERSION: 1.0
'* HOME URL: http://www.dspdimension.com
'* KNOWN BUGS: none
'*
'* SYNOPSIS: Routine for doing pitch shifting while maintaining
'* duration using the Short Time Fourier Transform.
'*
'* DESCRIPTION: The routine takes a pitchShift factor value which is between 0.5
'* (one octave down) and 2. (one octave up). A value of exactly 1 does not change
'* the pitch. numSampsToProcess tells the routine how many samples in indata[0...
'* numSampsToProcess-1] should be pitch shifted and moved to outdata[0 ...
'* numSampsToProcess-1]. The two buffers can be identical (ie. it can process the
'* data in-place). fftFrameSize defines the FFT frame size used for the
'* processing. Typical values are 1024, 2048 and 4096. It may be any value <=
'* MAX_FRAME_LENGTH but it MUST be a power of 2. osamp is the STFT
'* oversampling factor which also determines the overlap between adjacent STFT
'* frames. It should at least be 4 for moderate scaling ratios. A value of 32 is
'* recommended for best quality. sampleRate takes the sample rate for the signal 
'* in unit Hz, ie. 44100 for 44.1 kHz audio. The data passed to the routine in 
'* indata[] should be in the range [-1.0, 1.0), which is also the output range 
'* for the data, make sure you scale the data accordingly (for 16bit signed integers
'* you would have to divide (and multiply) by 32768). 
'*
'* COPYRIGHT 1999-2006 Stephan M. Bernsee <smb [AT] dspdimension [DOT] com>
'*
'* 						The Wide Open License (WOL)
'*
'* Permission to use, copy, modify, distribute and sell this software and its
'* documentation for any purpose is hereby granted without fee, provided that
'* the above copyright notice and this license appear in all source copies. 
'* THIS SOFTWARE IS PROVIDED "AS IS" WITHOUT EXPRESS OR IMPLIED WARRANTY OF
'* ANY KIND. See http://www.dspguru.com/wol.htm for more information.
'*
'****************************************************************************


'***************************************************************************
'*
'* This code was converted to C# by Michael Knight
'* madmik3 at gmail dot com. 
'* http://sites.google.com/site/mikescoderama/
'*
'****************************************************************************


Imports System.Collections.Generic
Imports System.Text

Namespace Mike.Rules
    Public Class PitchShifter

#Region "Private Static Memebers"
        Private Shared MAX_FRAME_LENGTH As Integer = 16000
        Private Shared gInFIFO As Single() = New Single(MAX_FRAME_LENGTH - 1) {}
        Private Shared gOutFIFO As Single() = New Single(MAX_FRAME_LENGTH - 1) {}
        Private Shared gFFTworksp As Single() = New Single(2 * MAX_FRAME_LENGTH - 1) {}
        Private Shared gLastPhase As Single() = New Single(MAX_FRAME_LENGTH / 2) {}
        Private Shared gSumPhase As Single() = New Single(MAX_FRAME_LENGTH / 2) {}
        Private Shared gOutputAccum As Single() = New Single(2 * MAX_FRAME_LENGTH - 1) {}
        Private Shared gAnaFreq As Single() = New Single(MAX_FRAME_LENGTH - 1) {}
        Private Shared gAnaMagn As Single() = New Single(MAX_FRAME_LENGTH - 1) {}
        Private Shared gSynFreq As Single() = New Single(MAX_FRAME_LENGTH - 1) {}
        Private Shared gSynMagn As Single() = New Single(MAX_FRAME_LENGTH - 1) {}
        Private Shared gRover As Long, gInit As Long
#End Region

#Region "Public Static  Methods"
        Public Shared Sub PitchShift(pitchShift__1 As Single, numSampsToProcess As Long, sampleRate As Single, indata As Single())
            PitchShift(pitchShift__1, numSampsToProcess, CLng(2048), CLng(10), sampleRate, indata)
        End Sub
        Public Shared Sub PitchShift(pitchShift__1 As Single, numSampsToProcess As Long, fftFrameSize As Long, osamp As Long, sampleRate As Single, indata As Single())
            Dim magn As Double, phase As Double, tmp As Double, window As Double, real As Double, imag As Double
            Dim freqPerBin As Double, expct As Double
            Dim i As Long, k As Long, qpd As Long, index As Long, inFifoLatency As Long, stepSize As Long,
                fftFrameSize2 As Long


            Dim outdata As Single() = indata

            fftFrameSize2 = fftFrameSize \ 2
            stepSize = fftFrameSize \ osamp
            freqPerBin = sampleRate / CDbl(fftFrameSize)
            expct = 2.0 * Math.PI * CDbl(stepSize) / CDbl(fftFrameSize)
            inFifoLatency = fftFrameSize - stepSize
            If gRover = 0 Then
                gRover = inFifoLatency
            End If


            For i = 0 To numSampsToProcess - 1

                gInFIFO(gRover) = indata(i)
                outdata(i) = gOutFIFO(gRover - inFifoLatency)
                gRover += 1


                If gRover >= fftFrameSize Then
                    gRover = inFifoLatency

                    For k = 0 To fftFrameSize - 1
                        window = -0.5 * Math.Cos(2.0 * Math.PI * CDbl(k) / CDbl(fftFrameSize)) + 0.5
                        gFFTworksp(2 * k) = CSng(gInFIFO(k) * window)
                        gFFTworksp(2 * k + 1) = 0F
                    Next

                    ShortTimeFourierTransform(gFFTworksp, fftFrameSize, -1)

                    For k = 0 To fftFrameSize2

                        real = gFFTworksp(2 * k)
                        imag = gFFTworksp(2 * k + 1)


                        magn = 2.0 * Math.Sqrt(real * real + imag * imag)
                        phase = Math.Atan2(imag, real)

                        tmp = phase - gLastPhase(k)
                        gLastPhase(k) = CSng(phase)

                        tmp -= CDbl(k) * expct


                        qpd = CLng(Math.Truncate(tmp / Math.PI))
                        If qpd >= 0 Then
                            qpd += qpd And 1
                        Else
                            qpd -= qpd And 1
                        End If
                        tmp -= Math.PI * CDbl(qpd)


                        tmp = osamp * tmp / (2.0 * Math.PI)

                        tmp = CDbl(k) * freqPerBin + tmp * freqPerBin


                        gAnaMagn(k) = CSng(magn)

                        gAnaFreq(k) = CSng(tmp)
                    Next


                    For zero As Integer = 0 To fftFrameSize - 1
                        gSynMagn(zero) = 0
                        gSynFreq(zero) = 0
                    Next

                    For k = 0 To fftFrameSize2
                        index = CLng(Math.Truncate(k * pitchShift__1))
                        If index <= fftFrameSize2 Then
                            gSynMagn(index) += gAnaMagn(k)
                            gSynFreq(index) = gAnaFreq(k) * pitchShift__1
                        End If
                    Next

                    For k = 0 To fftFrameSize2

                        magn = gSynMagn(k)
                        tmp = gSynFreq(k)

                        tmp -= CDbl(k) * freqPerBin

                        tmp /= freqPerBin

                        tmp = 2.0 * Math.PI * tmp / osamp

                        tmp += CDbl(k) * expct

                        gSumPhase(k) += CSng(tmp)
                        phase = gSumPhase(k)

                        gFFTworksp(2 * k) = CSng(magn * Math.Cos(phase))
                        gFFTworksp(2 * k + 1) = CSng(magn * Math.Sin(phase))
                    Next

                    For k = fftFrameSize + 2 To 2 * fftFrameSize - 1
                        gFFTworksp(k) = 0F
                    Next

                    ShortTimeFourierTransform(gFFTworksp, fftFrameSize, 1)

                    For k = 0 To fftFrameSize - 1
                        window = -0.5 * Math.Cos(2.0 * Math.PI * CDbl(k) / CDbl(fftFrameSize)) + 0.5
                        gOutputAccum(k) += CSng(2.0 * window * gFFTworksp(2 * k) / (fftFrameSize2 * osamp))
                    Next
                    For k = 0 To stepSize - 1
                        gOutFIFO(k) = gOutputAccum(k)
                    Next

                    For k = 0 To fftFrameSize - 1
                        gOutputAccum(k) = gOutputAccum(k + stepSize)
                    Next

                    For k = 0 To inFifoLatency - 1
                        gInFIFO(k) = gInFIFO(k + stepSize)
                    Next
                End If
            Next
        End Sub
#End Region

#Region "Private Static Methods"
        Public Shared Sub ShortTimeFourierTransform(fftBuffer As Single(), fftFrameSize As Long, sign As Long)
            Dim wr As Single, wi As Single, arg As Single, temp As Single
            Dim tr As Single, ti As Single, ur As Single, ui As Single
            Dim i As Long, bitm As Long, j As Long, le As Long, le2 As Long, k As Long

            For i = 2 To 2 * fftFrameSize - 3 Step 2
                bitm = 2
                j = 0
                While bitm < 2 * fftFrameSize
                    If (i And bitm) <> 0 Then
                        j += 1
                    End If
                    j <<= 1
                    bitm <<= 1
                End While
                If i < j Then
                    temp = fftBuffer(i)
                    fftBuffer(i) = fftBuffer(j)
                    fftBuffer(j) = temp
                    temp = fftBuffer(i + 1)
                    fftBuffer(i + 1) = fftBuffer(j + 1)
                    fftBuffer(j + 1) = temp
                End If
            Next
            Dim max As Long = CLng(Math.Truncate(Math.Log(fftFrameSize) / Math.Log(2.0) + 0.5))
            k = 0
            le = 2
            While k < max
                le <<= 1
                le2 = le >> 1
                ur = 1.0F
                ui = 0F
                arg = CSng(Math.PI) / (le2 >> 1)
                wr = CSng(Math.Cos(arg))
                wi = CSng(sign * Math.Sin(arg))
                For j = 0 To le2 - 1 Step 2

                    i = j
                    While i < 2 * fftFrameSize
                        tr = fftBuffer(i + le2) * ur - fftBuffer(i + le2 + 1) * ui
                        ti = fftBuffer(i + le2) * ui + fftBuffer(i + le2 + 1) * ur
                        fftBuffer(i + le2) = fftBuffer(i) - tr
                        fftBuffer(i + le2 + 1) = fftBuffer(i + 1) - ti
                        fftBuffer(i) += tr

                        fftBuffer(i + 1) += ti
                        i += le
                    End While
                    tr = ur * wr - ui * wi
                    ui = ur * wi + ui * wr
                    ur = tr
                Next
                k += 1
            End While
        End Sub
#End Region
    End Class
End Namespace
