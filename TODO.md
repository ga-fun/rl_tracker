# TODO

## CORE

1. Move `Notifier` to a dedicated *project* to use it anywhere without circular references

2. `src/RlStatsApi/Config.cs` => Inherit from `Notifier`

3. **SPLIT** `src/FileIni/*.cs` en (*internal*):
- `FinelIniReader` (fonction `Read()` + helpers)
- `FileIniWriter` (fonction `Write()` + helpers)
- `FileIniNormalizer` (fonctions `Normalize*()`)

4. **MERGE** ce qui touche à *Rocket League* dans un *projet* `src/RocketLeague`:
- `src/RlTracker.Core/Config/RlInstall*.cs`
- `src/RlTracker.Core/Utils/RlProcess.cs`
- `src/RlStatsApi/**/*.cs`

## UI

### Components

- `ConnectionStatus`:
	- [OUT] string `Status` (red = disconnected / yellow = connecting / green = connected)
- `MainTracker`:
	- [OUT] uint `WinCount`
	- [OUT] uint `LossCount`
	- [OUT] int `StreakCount`
- [BUTTON] `SettingsButton`
- `UiSettings`:
	- [IN] string `WinPrefix`
	- [IN] string `LossPrefix`
	- [IN] string `WinStreakPrefix`
	- [IN] string `LossStreakPrefix`
- `InstallSettings`:
	- [IN] string `EpicRlInstallDir`
	- [OUT] string `EpicStatus`
	- [IN] string `SteamRlInstallDir`
	- [OUT] string `SteamStatus`
- `ClientSettings`:
	- [IN] int `Port`
	- [IN] double `PacketSendRate`
- `TrackerCount`:
	- [BUTTON] `Minus`
	- [OUT] int `Count`
	- [BUTTON] `Plus`
- `TrackerSettings`:
	- [CHECKBOX] `IsActive`
	- [OUT] string `GameMode`
	- `TrackerCount` `Win`
	- `TrackerCount` `Lose`
	- [OUT] int `Streak`
	- [BUTTON] `Reset`
- `LogsDisplay`
	- [OUT] string `Logs`


### Windows

- `MinimalWindow`:
	- `MainTracker`
	- `SettingsButton`
- `ConfigWindow`:
	- `UiSettings`
	- `InstallSettings`
	- `ClientSettings`

### Flow

1. If `RlNotFound == true` => only show `ConfigWindow`
2. Else => show `MinimalWindow`
3. When `SettingsButton` is clicked: show `SettingsWindow`

- Catch + error messages:
	- `⚠️ Config not found: initializing default config`
	- `⚠️ Unable to save config: {exception.message}`
	- `⚠️ Unable to find Rocket League (Epic version)`
	- `⚠️ Unable to find Rocket League (Steam version)`
	- `⚠️ Config updated: please start or restart Rocket League`
