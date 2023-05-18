# Windows Script Compiler Collection
A collection of compilers/converters for Windows script languages such as VBScript, JScript and WSF.

## Windows Script Compiler (wsc)
A program to convert VBScript and JScript (JavaScript) files to an executable file.

### Features:
* Checks if the script is backwards-compatible with JScript .NET, then compiles it with the JScript .NET compiler if it is.
* Support for WScript and CScript

### Command Line Options:
```
GyWare WSCC Windows Script Compiler [https://github.com/gyware/wscc]
Copyright (C) GyWare. All rights reserved.
			
Usage: wsc.wsf filename [/nocompile[+|-]] [/engine:value] [/nologo[+|-]] [/target:value]

Options:

filename  : The script file to process
nocompile : Don't automatically compile the output JavaScript code with the JScript .NET compiler
engine    : The script engine to execute the script with, usually the language e.g. VBScript, JScript (default: detected by file extension)
nologo    : Don't display a copyright banner in standard output
target    : The script host to execute the script with e.g. WScript, CScript (default: WScript)
Example   : wsc C:\Path\To\Script.vbs /engine:VBScript /target:WScript
```

## VBScript to JavaScript Converter (vbjs)
A program to convert VBScript to JavaScript.
