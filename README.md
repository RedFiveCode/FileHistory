# FileHistoryApp

## Introduction
Windows 10 has a [file history](https://support.microsoft.com/en-us/windows/file-history-in-windows-5de0e203-ebae-05ab-db85-d5aa0a199255)/backup feature whereby generations of files in selected folders are archived to a destination folder or drive.
See https://support.microsoft.com/en-us/windows/file-history-in-windows-5de0e203-ebae-05ab-db85-d5aa0a199255

Windows has an existing graphical feature to allow the users to [review and restore](https://support.microsoft.com/en-us/windows/backup-and-restore-in-windows-352091d2-bb9d-3ea3-ed18-52ef2b88cbef) selected archived files.

This application allows the user to list and delete generations of files in an archive folder. The archive folder does not have to be that belonging to the current PC or user.
Unlike the Windows feature, it is not restricted to viewing the current user's archive or risking changes to the current archive configuration.

Possible operations are:
* list file generations; can filter files, optionally recurse sub-folders
* delete old file generations; can optionally filter files, keep the last _N_ generations and preview file names before deletion

## Background
This application arose as I had several instances of FileHistory backups from several old PCs and users accumulated over time.
The goal was to provide a consolidated view of each archive and so reduce the required disc space by deleting older generations of each file.


## Technical
This is a command line application written in C# for the .net framework version 4.8.

It uses the following nuget packages/files:
* [ByteSize](https://github.com/omar/ByteSize)
* [Command Line Parser](https://github.com/commandlineparser/commandline)
* [System-IO-Abstractions](https://github.com/System-IO-Abstractions/System.IO.Abstractions)
* [ColorConsole](https://gist.github.com/RickStrahl/52c9ee43bd2723bcdf7bf4d24b029768)

Includes unit tests (MSTest).

## Examples
Command line help is available:

```
FileHistory --help
  show       (Default Verb) Show file history files
  tidy       Tidy old file history files
  help       Display more information on a specific command.
  version    Display version information.
```

The _list_ command is to find and display old generations of matching files in the specified folder:
```
FileHistory list --help
  -h, --hide           Hide display of files with only one generation (and no history).
  -f, --folder         Required. Input folder.
  -r, --recurse        Recurse sub-folders.
  -m, --match          Filter using filename wildcard (case insensitive).
  -g, --minimumSize    Filter to include files greater than minimum size.
  -v, --verbose        Set output to verbose messages.
  --help               Display this help screen.
  --version            Display version information.
```

The _tidy_ command is to delete old versions of matching files in the specified folder:
```
FileHistory tidy --help
  -p, --preview                  Preview, do not delete files.
  -k, --keepRecentGenerations    (Default: 1) Keep the most recent N generations
  -f, --folder                   Required. Input folder.
  -r, --recurse                  Recurse sub-folders.
  -m, --match                    Filter using filename wildcard (case insensitive).
  -g, --minimumSize              Filter to include files greater than minimum size.
  -v, --verbose                  Set output to verbose messages.
  --help                         Display this help screen.
  --version                      Display version information.
```


## Testing
Tested with Windows 10 Professional version 20H2.
Includes unit tests.

## Security considerations
Enumerates files and folders.
Display file name, path and timestamp information.
Deletes files.


## Author and License
RedFiveCode (https://github.com/RedFiveCode)

Copyright (c) 2021 RedFiveCode (https://github.com/RedFiveCode) All rights reserved.

Released under MIT License (see License.txt file).

## Acknowledgments
Windows is a trademark of the Microsoft group of companies.



