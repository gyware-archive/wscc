Imports System.Diagnostics
Imports System.IO

Module Module1
    Sub Main(args As String())
        On Error GoTo ErrorHandler

        Dim argTarget As String = GetArgumentValue(args, "target")
        If argTarget = "" Then
            argTarget = "wscript"
        End If

        Dim argEngine As String = GetArgumentValue(args, "engine")

        Call compileScript(args(0), argTarget, argEngine)

ErrorHandler:
        If Err.Number <> 0 Then
            Console.WriteLine("Error #" & Str(Err.Number) & ": " & Err.Description)
        End If
    End Sub

    Function compileScript(scriptPath As String, scriptTarget As String, scriptEngine As String)
        Dim scriptContent As String = File.ReadAllText(scriptPath)

        Dim tempDir As String = Path.GetTempPath()
        Dim jsPath As String = Path.Combine(tempDir, Path.GetRandomFileName())

        Dim output As String

        If scriptEngine <> "" Then
            output = "import System;import System.IO;import System.Diagnostics;var args:String[]=Environment.GetCommandLineArgs();var c='" & Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(scriptContent, "\r", "\\r"), "\n", "\\n"), "\r\n", "\\r\\n"), "'", "\'"), vbCr, "\r"), vbLf, "\n"), vbCrLf, "\r\n"), "\\", "\\\\") & "',f=Path.GetTempFileName();File.WriteAllText(f,c);var p=Process.Start('" & scriptTarget & "','//nologo //e:" & scriptEngine & " '+f+' '+args.join(' '));p.WaitForExit();File.Delete(f);"
        Else
            output = "import System;import System.IO;import System.Diagnostics;var args:String[]=Environment.GetCommandLineArgs();var c='" & Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(scriptContent, "\r", "\\r"), "\n", "\\n"), "\r\n", "\\r\\n"), "'", "\'"), vbCr, "\r"), vbLf, "\n"), vbCrLf, "\r\n"), "\\", "\\\\") & "',f=Path.ChangeExtension(Path.Combine(Path.GetTempPath(),Path.GetRandomFileName()), '" & Path.GetExtension(scriptPath) & "');File.WriteAllText(f,c);var p=Process.Start('" & scriptTarget & "','//nologo '+f+' '+args.join(' '));p.WaitForExit();File.Delete(f);"
        End If

        File.WriteAllText(jsPath, output)

        Dim args As String() = Environment.GetCommandLineArgs
        Dim out As String = GetArgumentValue(args, "out")
        If out = "" Then
            out = Path.Combine(Path.GetDirectoryName(scriptPath), Path.GetFileNameWithoutExtension(scriptPath) & ".exe")
        Else
            out = Path.GetFullPath(out)
        End If

		Dim jsc = New Process()
		jsc.StartInfo.FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Microsoft.NET", "Framework64", "v4.0.30319", "jsc.exe")
		jsc.StartInfo.Arguments = "/nologo /target:winexe /out:" & out & " " & jsPath
		jsc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        jsc.Start()
		jsc.WaitForExit()
		
        File.Delete(jsPath)
        Return ""
    End Function

    Function GetArgumentValue(args As String(), argName As String) As String
        For Each arg In args
            If arg.StartsWith("/") AndAlso arg.Contains(":"c) Then
                Dim argParts As String() = arg.Substring(1).Split(":")
                If argParts(0).ToLower() = argName.ToLower() Then
                    Return argParts(1)
                End If
            End If
        Next
        Return ""
    End Function
End Module
