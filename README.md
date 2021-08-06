# Changey

![Nuget](https://img.shields.io/nuget/v/Changey) 
[![CI Build](https://github.com/TheSylence/Changey/actions/workflows/ci.yml/badge.svg)](https://github.com/TheSylence/Changey/actions/workflows/ci.yml)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/9d1b30c3f4014291af9b8a9b2791d829)](https://www.codacy.com/gh/TheSylence/Changey/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=TheSylence/Changey&amp;utm_campaign=Badge_Grade)
![GitHub](https://img.shields.io/github/license/TheSylence/changey)

Manage your changelogs by adhering to the https://keepachangelog.com/en/1.1.0/ standard.
The generated changelogs are Markdown files that can be directly integrated into many webservices.

## Installation

Changey is distributed as a .NET tool and can be installed using the [dotnet tool install](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install) command:

`dotnet tool install --global Changey`

You can also download the the package from [Github releases](https://github.com/TheSylence/Changey/releases) or from [nuget](https://www.nuget.org/packages/Changey/). 

## Usage

### Global options

These options can be used with all commands:
* `-p` specifies the path to the changelog that should be used. The default is to use the `changelog.md` file in the current directory.
* `-s` disables all output to stdout and stderr.
* `-v` enables verbose logging output to stdout

### Creating a new changelog

`changey init` will initialize a new change log in the current directory.
This is required before using any other commands.
It will fail when the changelog already exists unless the `-o` option is set.

### Releasing a version

`changey release 1.2.3` will add a 1.2.3 as a released version to the changelog.

`changey release 1.2.3 -d 2020-05-13` will add 1.2.3 as a relased version on May the 13th, 2020 to the changelog.

You will be warned (and the relase will be aborted) when you are trying to release a version that is older than a previously released version.
I.e. when there is a released version 1.2.1 and you try to release a version 1.2.0 it will fail.

You can force a release by specifing the `-f` flag:

`changey release 1.2.0 -f`

### Adding changes

`changey add "Added a cool feature"` will add "Added a cool feature" under the "Add" section to the changelog. (Make sure to suround your message with quotes if it contains spaces.)

List of possible sections to add to:
* add
* fix
* change
* remove
* deprecate
* security

Use them as a verb when invoking changey:
`changey change` to add to the "Change" section, `changey deprecate` for the "Deprecate" section and so on.

### Extracting parts of a changelog

The changes for a single version can be extracted to a file that can be used to document changes of a single version.
This is useful when informing users of a new release for example.

`changey extract 1.2.3 -t changes_1.2.3.md`

Will write all changes for version 1.2.3 from the current changelog to a file called *changes_1.2.3.md*

Specify the `-h` flag if you want to include the name and release date of the version in the generated file.

### Yanking a release version

`changey yank` will mark the last released version as yanked.

### Help

`changey help` and `changey help [command]` will output help texts and usages for every command available.

## Build from source

You can either use `dotnet build` (.NET 5.0 SDK required) to build the sources or use Visual Studio 2019 16.8 or later (or Rider 2021.2 or later) and open the Changey.sln file.