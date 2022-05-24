# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Added
- Add support for compare URLs to view changes between versions (#28)

## [0.3] - 2021-08-06
### Added
- New verb 'extract' to extract sections of a changelog to a file (#22)

### Changed
- Hide exception stacktraces unless verbose logging is enabled (#21)
- Warn when trying to release version that is older than a previous released version (#23)

### Fixed
- Ensure versions are sorted by release date in changelog (#26)

## [0.2] - 2021-07-29
### Added
- Usage examples for verbs in help screen (#10)

### Fixed
- Adding to sections fails after first release (#24)

## [0.1.2] - 2021-07-29
### Fixed
- Display correct version when invoking tool (#20)


[Unreleased]: https://github.com/TheSylence/changey/compare/0.3...HEAD
[0.3]: https://github.com/TheSylence/changey/compare/0.2...0.3
[0.2]: https://github.com/TheSylence/changey/compare/0.1.2...0.2
[0.1.2]: https://github.com/TheSylence/changey/releases/tag/0.1.2
<!-- Release: %URL%/releases/tag/%VERSION% -->
<!-- Compare: %URL%/compare/%OLD_VERSION%...%NEW_VERSION% -->
<!-- BaseUrl: https://github.com/TheSylence/changey -->
