## overview

This is a Unity SDK wrapper for the JustTrack SDK by justtrack GmbH.

## sources

- `JustTrackSDK_Wrapper/` - Main Unity SDK project (published as Unity package `io.justtrack.justtrack-unity-sdk`)
- `TestApp/` - Example/demo application with full SDK integration
- `browser-sdk/` - Browser/WebGL SDK (git submodule)
- `tracking-sdk/` - Native iOS/Android SDK (git submodule)
- `tools/` - Build tools for dependency and adapter version generation
- `scripts/` - Helper scripts (linting, testing, credentials)

## tools

- `dotnet format analyzers JustTrackSDK_Wrapper/JustTrackSDK.csproj` - runs StyleCop analyzers on the SDK code
- `sh scripts/lint.sh` - runs linter (dotnet format with SA1600 check)
- `sh scripts/run-editor-tests.sh` - runs Editor (Edit Mode) unit tests from CLI
- `sh scripts/run-editor-tests.sh --coverage` - runs Editor unit tests with code coverage report
- `sh scripts/generateCredentials.sh` - generates credential files from `local.properties`
- `./updateCode.sh` - checks out submodules, builds browser SDK, updates versions and dependencies

## commands

- `sh scripts/lint.sh` - runs linter
- `sh scripts/run-editor-tests.sh --coverage` - runs Editor unit tests with code coverage report

## instructions

- always check code with linter, formatting and tests after you're done
