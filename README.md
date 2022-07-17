# Furtiv Console App Wrapper

Furtiv is a console application wrapper for Windows. It's intended to launch command line tools (like PowerShell scripts) without displaying a window. Usefull for startup scripts and triggered user actions.

## How to use

To launch a command line application, simply add the command line after *furtiv.exe*:

    Furtiv.exe gpupdate / force

To launch a PowerShell command, use the switch *-PowerShell* and simply append the command(s) you want to launch

    Furtiv.exe -PowerShell New-Item -Path C:\Temp\test -ItemType File

Paramters :

* -Help (or -h): display help message
* -LogFolder path_to_log: create logs in specified log folder
* -PowerShell: use PowerShell interpreter for the command
* -EncodedCommand: use PowerShell EncodedCommand argument (see PowerShell section bellow)

Parameters must all be placed at the beginning of the line.

Everything placed after parameters will be executed as a command line.

### PowerShell

When using PowerShell, this is the full command line used to launch your command:

    PowerShell.exe -ExecutionPolicy Unrestricted -NoLogo -WindowStyle Hidden -Command xxx

In some complex cases, the command line misinterpreted (bad escaping, special chars misinterpreted). One solution is to encode the whole command line in base 64 and use the EncodedCommand PowerShell parameter. To do so with Furtiv, place the *-EncodedCommand* argument after the *-PowerShell*. Example with the 'Get-Process' cmdlet:

    Furtive.exe -PowerShell -EncodedCommand RwBlAHQALQBQAHIAbwBjAGUAcwBzAA==

To get your command base 64 encoded, use the following PowerShell statement:

    [Convert]::ToBase64String([System.Text.Encoding]::Unicode.GetBytes("your_command"))

## Logging

Furtiv create at least one log file to trace what is launched and when. By default logs file are located in the folder *AppData\Local\Furtiv\Logs* under the user profile.

You can change the log folder with parameter *-logfolder path_to_logfolder*:

    furtiv.exe -logfolder C:\Temp\myLogs gpupdate / force

Example logs with the command **furtiv.exe ping 127.0.0.1**:

    2022.07.15 21:20:27 - 7008 - Launching Console App 'ping' with arguments: '127.0.0.1'
    2022.07.15 21:20:27 - 7008 - Successfully created Console App process with ID 16152
    2022.07.15 21:20:30 - 7008 - Console App exited with code 0

Furtiv also create a log file with Standard Output (stdout) of the command. Fields are:

* the date
* the process ID of the console application
* the line captured on stdout

Example with ping:

    2022.07.15 21:20:27 - 7008 - Pinging 127.0.0.1 with 32 bytes of data:
    2022.07.15 21:20:27 - 7008 - Reply from 127.0.0.1: bytes=32 time<1ms TTL=128
    2022.07.15 21:20:28 - 7008 - Reply from 127.0.0.1: bytes=32 time<1ms TTL=128
    2022.07.15 21:20:29 - 7008 - Reply from 127.0.0.1: bytes=32 time<1ms TTL=128
    2022.07.15 21:20:30 - 7008 - Reply from 127.0.0.1: bytes=32 time<1ms TTL=128
    2022.07.15 21:20:30 - 7008 - Ping statistics for 127.0.0.1:
    2022.07.15 21:20:30 - 7008 -     Packets: Sent = 4, Received = 4, Lost = 0 (0% loss),
    2022.07.15 21:20:30 - 7008 - Approximate round trip times in milli-seconds:
    2022.07.15 21:20:30 - 7008 -     Minimum = 0ms, Maximum = 0ms, Average = 0ms

And, if necessary, a log file with Standard Error (stderr) of the command. Example with a non existent PowerShell command:

    2022.07.17 13:24:45 - 8152 - Error - Non-ExistentCommand : The term 'Non-ExistentCommand' is not recognized as the name of a cmdlet, function, script file, 
    2022.07.17 13:24:45 - 8152 - Error - or operable program. Check the spelling of the name, or if a path was included, verify that the path is correct and 
    2022.07.17 13:24:45 - 8152 - Error - try again.
    2022.07.17 13:24:45 - 8152 - Error - At line:1 char:1
    2022.07.17 13:24:45 - 8152 - Error - + Non-ExistentCommand
    2022.07.17 13:24:45 - 8152 - Error - + ~~~~~~~~~~~~~~~~~~~
    2022.07.17 13:24:45 - 8152 - Error -     + CategoryInfo          : ObjectNotFound: (Non-ExistentCommand:String) [], CommandNotFoundException
    2022.07.17 13:24:45 - 8152 - Error -     + FullyQualifiedErrorId : CommandNotFoundException

Note for PowerShell commands: when powerShell is launched without a terminal, stderr is encoded using CLIXML. Furtiv do a basic decoding of CLIXML and keep only strings. Warning or Error is placed before the log line depending of the 'S' field in CLIXML. Stdout is not encoded, so you get the same content in logs as on screen.

## How to code

This application is built with Visual Studio Community with C# WinForm features.

## References and thanks

This app is based on the [Doing a NotifyIcon Program the Right Way](https://www.codeproject.com/Tips/627796/Doing-a-NotifyIcon-Program-the-Right-Way) Code Project article by [Johnny J.](https://www.codeproject.com/script/Membership/View.aspx?mid=926948)

Furtiv icon comes from [Free ICONS Library](https://icon-library.com/icon/icon-f-13.html)
