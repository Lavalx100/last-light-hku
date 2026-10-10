# Last Light: HKU

A first-person exploration game set in a darkened recreation of the HKU Main Building. Explore, complete four console mini-games, collect relics, and reach the exit.

## Play the game

Download the latest macOS build from [GitHub Releases](https://github.com/Lavalx100/last-light-hku/releases/latest/download/LastLightHKU-macOS.zip), unzip it, and open **Last Light HKU.app**. The student build is not notarized, so macOS may ask you to approve it in **System Settings → Privacy & Security**.

For the controls, objective, mini-game instructions, and screenshots, see the [gameplay walkthrough](docs/GAMEPLAY.md).

## Controls

- **W / A / S / D** — Move
- **Mouse** — Look around and use the phone interface
- **Tab** — Open or close the phone
- **F** — Toggle the flashlight
- **E** — Start a nearby console or interact with an object
- **Esc** — Open the in-game menu

The phone battery drains while the phone is open or the flashlight is on. The phone's Tasks page tracks the four console games and shows whether the exit is unlocked.

## Open the Unity project

1. Install Unity Editor **6000.3.6f1**.
2. Add the `LastLightHKU` folder in Unity Hub.
3. Open `Assets/Scenes/MainMenu.unity` to start from the title screen. `Assets/Scenes/MainGame.unity` is the gameplay scene.

The public scene uses original, simple placeholder geometry in place of omitted Asset Store prefabs and HKU merchandise scans. The collectible interactions, console games, task tracking, and exit logic remain available. These substitutes make the public scene and downloadable build usable without redistributing the excluded source assets. See [CREDITS.md](CREDITS.md) for the exact dependencies, attribution, and license notes.

To build a macOS app from the Unity Editor, run the `BuildPublicMacOS.Build` method in batch mode and set `LASTLIGHT_BUILD_OUTPUT` to an absolute `.app` output path. The helper is at `LastLightHKU/Assets/Editor/BuildPublicMacOS.cs`.

## Rights and licenses

The supplied project report identifies Jad Khiami as the game's author. This repository's [MIT License](LICENSE) covers the original game source only. Third-party works remain under their own licenses or publisher terms and are credited in [CREDITS.md](CREDITS.md); they are not relicensed by the MIT notice. The release excludes high-resolution merchandise scans whose redistribution rights were not established in the supplied project materials.

This is an independent student project and is not affiliated with or endorsed by the University of Hong Kong. University names and marks remain the property of their respective owners.
