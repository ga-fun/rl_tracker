# Ignore replays

In `UpdateState` if `bReplay == true` => ignore it

# TODO

- `RlStatsApi.Config.Update()`
- ``

- Catch + error messages:
	- `⚠️ Config not found: initializing default config`
	- `⚠️ Unable to save config: {exception.message}`
	- `⚠️ Unable to find Rocket League (Epic version)`
	- `⚠️ Unable to find Rocket League (Steam version)`
	- `⚠️ Config updated: please start or restart Rocket League`

1. Connect
2. Read full message

# Flow

1. Load `Core.Config`
2. if `EpicRlDir` and `SteamRlDir` == null:
	- Ask user to give RlInstallDir (input + `browse` button)
