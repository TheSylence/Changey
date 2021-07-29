# Changey

![Nuget](https://img.shields.io/nuget/v/Changey) 
[![CI Build](https://github.com/TheSylence/Changey/actions/workflows/ci.yml/badge.svg)](https://github.com/TheSylence/Changey/actions/workflows/ci.yml)
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

### Adding changes

`changey add "Added a cool feature"` will add "Added a cool feature" under the "Add" section to the changelog.
List of possible sections:
* add
* fix
* change
* remove
* deprecate
* security

### Yanking a release version

`changey yank` will mark the last released version as yanked.

### Help

`changey help` and `changey help [command]` will output help texts and usages for every command available.

## Build from source

You can either use `dotnet build` (.NET 5.0 SDK required) to build the sources or use Visual Studio 2019 16.8 or later (or Rider 2021.2 or later) and open the Changey.sln file.