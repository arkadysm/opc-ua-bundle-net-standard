Option Explicit

Dim shell, fso
Dim projectPath, rootDir, dotnetVersion, srcFile, dstFile, exeFile, configFile, netFwFile

Set shell = WScript.CreateObject("WScript.Shell")

If WScript.Arguments.Count > 0 Then
    projectPath = WScript.Arguments(0)
    WScript.Echo "Project Path: " & projectPath 
End If

If WScript.Arguments.Count > 1 Then
    rootDir = WScript.Arguments(1)
    WScript.Echo "Root Dir: " & rootDir 
End If

If WScript.Arguments.Count > 2 Then
    dotnetVersion = WScript.Arguments(2)
    WScript.Echo ".NET Version: " & dotnetVersion 
End If

WScript.Echo "" 

Set fso = CreateObject("Scripting.FileSystemObject")

exeFile = rootDir & "bin\net" & dotnetVersion & "\Technosoftware.UaServer.exe"
  
If fso.FileExists(exeFile) Then

  Set srcFile = fso.GetFile(exeFile)
  srcFile.Copy projectPath & "Technosoftware.UaServer.exe"  

  WScript.Echo "Copied Exe File: " & exeFile

End If

configFile = rootDir & "bin\net" & dotnetVersion & "\Technosoftware.UaServer.exe.config"

If fso.FileExists(configFile) Then

  Set srcFile = fso.GetFile(configFile)
  srcFile.Copy projectPath & "Technosoftware.UaServer.exe.config"  

  WScript.Echo "Copied Config File: " & configFile

End If
