# justtrack Unity SDK

`JustTrackSDK_Wrapper/` is the Unity project for the published package `io.justtrack.justtrack-unity-sdk`. `TestApp/` is a demo integration that needs local setup this repo leaves out; test package changes in `JustTrackSDK_Wrapper/`.

After changing package code, run the EditMode tests.

- Package source: `JustTrackSDK_Wrapper/Assets/JustTrack/` holds `Runtime/`, `Editor/`, `Plugins/iOS/`, `Plugins/WebGL/`, `package.json` and `CHANGELOG.md`.
- Tests: `JustTrackSDK_Wrapper/Assets/_Tests/` (`Editor/`, `PlayMode/`). From the repository root, run the EditMode suite with the Unity Editor version in `JustTrackSDK_Wrapper/ProjectSettings/ProjectVersion.txt`, replacing `<Unity>` with that Editor's executable path (quoted or invoked as your shell requires):
  `<Unity> -batchmode -projectPath JustTrackSDK_Wrapper -runTests -testPlatform EditMode -testResults TestResults/editor-results.xml`
- Style: StyleCop rules come from `JustTrackSDK_Wrapper/stylecop.json` and `JustTrackSDK_Wrapper/.editorconfig`; public members need XML documentation (SA1600). The repo ships no lint project, so follow these rules by hand.
