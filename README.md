# Changey
Manage your changelogs by adhering to the https://keepachangelog.com/en/1.1.0/ standard.
The generated changelogs are Markdown files that can be directly integrated into many webservices.

## Usage

### Creating a new changelog

`changey init` will initialize a new change log in the current directory.
This is required before using any other commands

### Releasing a version

`changey release -n 1.2.3` will add a 1.2.3 as a released version to the changelog.

### Adding changes

`changey add -m "Added a cool feature"` will add "Added a cool feature" under the "Add" section to the changelog.
List of possible sections:
* add
* fix
* change
* remove
* deprecate
* security

### Yanking a release version

`changey yank` will mark the last released version as yanked.

## Build from source

You can either use `dotnet build` (.NET 5.0 SDK required) to build the sources or use Visual Studio 2019 16.8 or later and open the Changey.sln file.