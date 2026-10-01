# justtrack SDK

The [justtrack SDK](https://justtrack.io/) is a marketing SDK with full MMP (Mobile Measurement Partner) capabilities for your Unity application. It provides attribution, event tracking, ad revenue forwarding, in-app purchase tracking, retargeting, remote config, and more.

You can find the complete documentation at <https://docs.justtrack.io/sdk/overview/>.

## Requirements

| Property | Version |
|---|---|
| Unity | 2022.3.62f2+ |
| Android Min SDK | 21 (Android 5.0) |
| iOS Deployment Target | 12.0 |

## Installation

The justtrack SDK is available as a Unity package via npm.

### Using the Package Manager UI

1. Navigate to `Window` > `Package Manager`, then select `Advanced Project Settings` from the gear menu.
2. Add a scoped registry with the following settings:
   - **Name**: justtrack Package Registry
   - **URL**: `https://registry.npmjs.org`
   - **Scope(s)**: `io.justtrack`
3. Select `Packages: My Registries` in the drop-down menu, select the `justtrack SDK` and install it.

### Using manifest.json

Add the scoped registry and package directly to your `Packages/manifest.json`:

```json
{
  "dependencies": {
    "io.justtrack.justtrack-unity-sdk": "7.1.1"
  },
  "scopedRegistries": [
    {
      "name": "justtrack Package Registry",
      "url": "https://registry.npmjs.org",
      "scopes": [
        "io.justtrack"
      ]
    }
  ]
}
```

### Dependencies

The justtrack SDK requires the [External Dependency Manager for Unity](https://github.com/googlesamples/unity-jar-resolver) (at least version 1.2.167) to resolve native Android and iOS dependencies.

## Getting Started

### Adding the prefab

After installing the justtrack SDK and its dependencies, a new `justtrack` menu appears in Unity. Navigate to the first scene of your game and select `Create SDK instance` from the `justtrack` menu.

Select the instance of the prefab and add your API tokens for Android and iOS as applicable.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for details on the project structure, prerequisites, quality checks, and contribution workflow.

## Support

If you have any problems or issues with the SDK, feel free to reach out directly via [support@justtrack.io](mailto:support@justtrack.io).

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
