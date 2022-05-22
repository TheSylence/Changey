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

### Version links

To allow a user to view (code) changes between different version you can generate compare links for different versions.

`changey compare` will initialize this in an existing changelog and from then on will be used every time a new version is created or released.

You need to specify the base url of your project (`-b BASEURL`), a template for the generation of a URL for listing changes between two versions (`-c TEMPLATE`), and a template for an URL that points to a specific release (`-r TEMPLATE`).
If your project is hosted on a known platform you can ommit the templates since they will generated based on the base URL.
"Known platforms" are currently:
- github.com
- gitlab.com

So `changey compare github.com/TheSylence/Changey` will setup the changelog to generate URLs that point to comparisions between two versions (e.g. https://github.com/TheSylence/changey/compare/0.2...0.3) and URLs that point to a specific version (e.g. https://github.com/TheSylence/changey/releases/tag/0.3)

When using an "unknown" platform for hosting your project you need to specify a template for generating URLs to compare two versions.
This template is a single string with `%VARIABLES%` that will be replaced by the tool.

#### Compare Template

A compare template has the following variables:
- `%URL%` base URL of the project (e.g. https://github.com/TheSylence/changey)
- `%NEW_VERSION%` name of the later version (e.g. 0.1.2)
- `%OLD_VERSION%` name of the older version (e.g. 0.1.1)

For example the built in template for github is `%URL%/compare/%OLD_VERSION%...%NEW_VERSION%` 

#### Release Template 

A release template has the following variables:
- `%URL%` base URL of the project (e.g. https://github.com/TheSylence/changey)
- `%VERSION%` name of the version to link (e.g. 0.1.2)

For example the built in template for github is `%URL%/releases/tag/%VERSION%`

### Yanking a release version

`changey yank` will mark the last released version as yanked.

### Help

`changey help` and `changey help [command]` will output help texts and usages for every command available.

## Build from source

You can either use `dotnet build` (.NET 6.0 SDK required) to build the sources or use Visual Studio 2022 or later (or Rider 2022.1 or later) and open the Changey.sln file.