How to register assemble:

c:\Windows\Microsoft.NET\Framework\v4.0.30319>RegAsm.exe d:\TESTPRO\InteroperatingUnmanagedCode\InteroperatingLibrary\bin\Debug\PowerManagerLibrary.dll /tlb /codebase
c:\Windows\Microsoft.NET\Framework\v4.0.30319>RegAsm.exe e:\Mentoring\InteroperatingUnmanagedCode\InteroperatingUnmanagedCode\InteroperatingLibrary\bin\Debug\PowerManagerLibrary.dll /tlb /codebase

Script for VB macros:

Sub test()
Dim pw As PowerManager
Set pw = New PowerManagerLibrary.PowerManager
res = pw.GetPowerInfo_LastWakeTime
MsgBox res
End Sub
