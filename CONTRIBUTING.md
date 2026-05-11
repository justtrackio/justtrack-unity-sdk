# Contributing to justtrack SDK (Unity)

Thank you for your interest in contributing to the justtrack SDK. This document provides guidelines and instructions for contributing to this project.

## Project Structure

- `JustTrackSDK_Wrapper/` - Main Unity SDK project (published as a Unity package)
  - `Assets/JustTrack/Runtime/` - SDK runtime code (C#)
  - `Assets/JustTrack/Editor/` - Unity Editor extensions, code generation, dependency resolution
  - `Assets/JustTrack/Plugins/iOS/` - iOS native bridge code (Objective-C/Swift)
  - `Assets/JustTrack/Plugins/WebGL/` - WebGL bridge code (JavaScript)
  - `Assets/JustTrack/Prefabs/` - Pre-configured SDK prefab
  - `Assets/_Tests/` - Unit tests (Editor and Runtime)
- `TestApp/` - Example/demo application with full SDK integration
- `tools/` - Build tools (dependency and adapter version generation)
- `scripts/` - Helper scripts (linting, testing, credentials)

## Prerequisites

- Unity 2022.3 LTS or later (2022.3.62f3 recommended)
- .NET SDK (for `dotnet format` linting)
- [External Dependency Manager for Unity](https://github.com/googlesamples/unity-jar-resolver) (at least v1.2.167)
- Node.js (for build tools in `tools/`)
- Xcode (for iOS builds)
- Android SDK (for Android builds)
- CocoaPods (for iOS dependencies)

## Setup

1. Clone the repository and initialize the git submodules:

   ```bash
   git submodule init && git submodule update
   ```

2. Run the update script to check out the correct submodule versions, build the browser SDK, and update dependencies:

   ```bash
   ./updateCode.sh
   ```

3. Copy `local.properties` and configure it with your credentials. Then generate the credential files:

   ```bash
   sh scripts/generateCredentials.sh
   ```

4. Open the `JustTrackSDK_Wrapper` project in Unity.

## Code Quality

Before submitting changes, make sure your code passes all quality checks:

### Linting

```bash
sh scripts/lint.sh
```

This runs `dotnet format` with StyleCop analyzers and checks for missing XML documentation on public members.

### Running Tests

Editor (Edit Mode) unit tests:

```bash
sh scripts/run-editor-tests.sh
```

With code coverage report:

```bash
sh scripts/run-editor-tests.sh --coverage
```

## Building the Test App

### iOS

1. Select iOS as the target platform in Unity.
2. Build the iOS project to a directory.
3. Open the generated `Podfile` and verify that it contains the correct dependency versions.
4. Run `pod install` in the build directory.
5. Open the generated `.xcworkspace` file in Xcode.
6. To run on a device, configure signing in the **Signing and Capabilities** tab.

### iOS Framework Linkage

The justtrack SDK supports both static and dynamic framework linkage. Configure this in the Unity Editor:

1. Go to **Assets > External Dependency Manager > iOS Resolver > Settings**.
2. Toggle **Link frameworks statically** based on your requirements.

The SDK automatically detects the linkage setting from the generated Podfile and configures the Xcode project accordingly.

## How to Contribute

1. Fork the repository.
2. Create a feature branch from `main`:

   ```bash
   git checkout -b feature/your-feature-name
   ```

3. Make your changes.
4. Ensure all linting and tests pass.
5. Commit your changes with a clear, descriptive commit message.
6. Push your branch and open a merge request.

## Reporting Issues

If you find a bug or have a feature request, please open an issue with:

- A clear and descriptive title
- Steps to reproduce the issue (for bugs)
- Expected vs actual behavior
- SDK version, Unity version, and target platform (Android/iOS/WebGL)

## License

By contributing to this project, you agree that your contributions will be licensed under the [MIT License](LICENSE).
