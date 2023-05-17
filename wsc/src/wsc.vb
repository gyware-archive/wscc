Imports System.Diagnostics
Imports System.IO

Module Module1
    Sub Main(args As String())
        On Error GoTo ErrorHandler
		
		If Not args.Contains("/nologo", StringComparer.CurrentCultureIgnoreCase) AndAlso Not args.Contains("//nologo", StringComparer.CurrentCultureIgnoreCase) Then
			Console.WriteLine("GyWare WSCC Windows Script Compiler [https://github.com/gyware/wscc]")
			Console.WriteLine("Copyright (C) GyWare. All rights reserved.")
			Console.WriteLine()
		End If

        Dim argTarget As String = GetArgumentValue(args, "target")
        If argTarget = "" Then
            argTarget = "wscript"
        End If

        Dim argEngine As String = GetArgumentValue(args, "engine")

		Dim argPath As String = args(0)

        Call compileScript(argPath, argTarget, argEngine)

ErrorHandler:
        If Err.Number <> 0 Then
            Console.WriteLine("Error " & CStr(Err.Number) & ": " & Err.Description)
        End If
    End Sub

    Function compileScript(scriptPath As String, scriptTarget As String, scriptEngine As String)
        Dim scriptContent As String = File.ReadAllText(scriptPath)

        Dim tempDir As String = Path.GetTempPath()
        Dim jsPath As String = Path.Combine(tempDir, Path.GetRandomFileName())

        Dim output As String
		Dim exeTarget As String

		If Not scriptContent.ToLower().Contains("wscript") AndAlso Not scriptContent.ToLower().Contains("wsh") AndAlso scriptEngine.ToLower() = "jscript" Then
		output = scriptContent
		If scriptTarget.ToLower() = "cscript" Then
			exeTarget = "exe"
		Else
			exeTarget = "winexe"
		End If
        Else
		If scriptEngine <> "" Then
			output = "import System;import System.IO;import System.Diagnostics;var args:String[]=Environment.GetCommandLineArgs();var c='" & Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(scriptContent, "\r", "\\r"), "\n", "\\n"), "\r\n", "\\r\\n"), "'", "\'"), vbCr, "\r"), vbLf, "\n"), vbCrLf, "\r\n"), "\\", "\\\\") & "',f=Path.ChangeExtension(Path.Combine(Path.GetTempPath(),Path.GetRandomFileName()), '" & Path.GetExtension(scriptPath) & "');File.WriteAllText(f,c);var p=Process.Start('" & scriptTarget & "','//nologo //e:" & scriptEngine & " '+f+' '+args.join(' '));p.WaitForExit();File.Delete(f);"
        Else
			output = "import System;import System.IO;import System.Diagnostics;var args:String[]=Environment.GetCommandLineArgs();var c='" & Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(scriptContent, "\r", "\\r"), "\n", "\\n"), "\r\n", "\\r\\n"), "'", "\'"), vbCr, "\r"), vbLf, "\n"), vbCrLf, "\r\n"), "\\", "\\\\") & "',f=Path.ChangeExtension(Path.Combine(Path.GetTempPath(),Path.GetRandomFileName()), '" & Path.GetExtension(scriptPath) & "');File.WriteAllText(f,c);var p=Process.Start('" & scriptTarget & "','//nologo '+f+' '+args.join(' '));p.WaitForExit();File.Delete(f);"
        End If
		End If

        File.WriteAllText(jsPath, output)

        Dim args As String() = Environment.GetCommandLineArgs
        Dim out As String = GetArgumentValue(args, "out")
        If out = "" Then
            out = Path.ChangeExtension(Path.GetFullPath(scriptPath), ".exe")
        Else
            out = Path.GetFullPath(out)
        End If

		If Not GetArgumentValue(args, "autocompile").ToLower() = "false" Then
		Dim jscInfo = New ProcessStartInfo()
		jscInfo.FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Microsoft.NET", "Framework64", "v4.0.30319", "jsc.exe")
		jscInfo.Arguments = "/nologo /target:winexe /out:""" & out & """ " & jsPath
		jscInfo.CreateNoWindow = True
		jscInfo.UseShellExecute = False
        Dim jsc = Process.Start(jscInfo)
		jsc.WaitForExit()
		Else
		File.WriteAllText(Path.ChangeExtension(out, ".js"), output)
		End If
		
        File.Delete(jsPath)
        Return ""
    End Function

    Function GetArgumentValue(args As String(), argName As String) As String
        For Each arg In args
            If arg.StartsWith("/") AndAlso arg.Contains(":"c) Then
                Dim argParts As String() = arg.Substring(1).Split(":")
                If argParts(0).ToLower() = argName.ToLower() Then
                    Return Replace(argParts(1), """", "")
                End If
            End If
        Next
        Return ""
    End Function
End Module
