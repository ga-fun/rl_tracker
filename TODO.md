# TODO

## High priority

- UI => error message `Rocket League not found, please edit {ConfigFile}`
- UI => warning message `Rocket League config has been modified, please start or restart Rocket League`

## Low Priority

- UI => Refactor `MainTracker`
- Refactor `Config.Load()`
- `ApiMessageFramer` => clean code
- `RlNotFound` + `RlNeedRestart` => move to `RocketLeague.Config`
- Implement UI

## Build command for releases

```bash
dotnet publish ".\src\8_RlTracker.Ui\RlTracker.Ui.csproj" `
	-c Release `
	-r win-x64 `
	--self-contained true `
	-o ".\publish" `
	-p:PublishSingleFile=true `
	-p:IncludeNativeLibrariesForSelfExtract=true `
	-p:EnableCompressionInSingleFile=true `
	-p:DebugType=None `
	-p:DebugSymbols=false
```

---

# UI

## PREFIXES

- [ROW] = grouping horizontal un peu comme en flexbox
- [COL] = grouping vertical un peu comme en flexbox
- [IN] = affiche quelquechose qui peut être modifié par une saisie de l'utilisateur
- [OUT] = affiche quelquechose mais le contenu n'est pas éditable par l'utilisateur
- [BUTTON] = affiche un bouton cliquable

## COMPONENTS

- [COL] `InstallWindow`:
	- [OUT] string `Title`
	- [COL] string `EpicSection`:
		- [OUT] string `EpicTitle`
		- [ROW] `EpicRow`:
			- [IN] string epicInput
			- [BUTTON] `EpicSearchButton`
			- [BUTTON] `EpicBrowseButton`
		- [OUT] string `EpicStatus`
	- [COL] string `SteamSection`:
		- [OUT] string `SteamTitle`
		- [ROW] `SteamRow`:
			- [IN] string steamInput
			- [BUTTON] `SteamSearchButton`
			- [BUTTON] `SteamBrowseButton`
		- [OUT] string `SteamStatus`

- [COL] `MainTracker`:
	- [ROW] `TrackerContent`:
		- [OUT] string `Content` (WinPrefix + WinCount + separator + LossPrefix + LossCount + separator + StreakPrefix + StreakCount + separator)
		- [BUTTON] `SettingsButton`
	- [ROW] `TrackerStatus`:
		- [OUT] string `Status`

- [COL] `SettingsClosed`:
	- [ROW] `ConfigSection`:
		- [OUT] string `Title`
		- [OUT] string `EpicStatus`
		- [OUT] string `SteamStatus`
		- [BUTTON] `OpenButton`
	- [COL] `ConfigExpandableSection`:
		- TODO...
	- [ROW] `PlayerSection`:
		- [OUT] string `Title`
		- [OUT] string `PlayerName`
		- [BUTTON] `OpenButton`
	- [COL] `PlayerExpandableSection`:
		- TODO...
	- [ROW] `TrackerSection`:
		- [OUT] string `Title`
		- [OUT] string `Content`
		- [BUTTON] `OpenButton`
	- [COL] `TrackerExpandableSection`:
		- TODO...
	- [ROW] `LogsSection`:
		- [OUT] string `Title`
		- [OUT] string `ConnectionStatus`
		- [BUTTON] `OpenButton`
	- [COL] `LogExpandableSection`:
		- TODO...

## Flow

1. If `RlNotFound == true` => only show `InstallWindow`
2. Else => show `MainTracker`
3. When `MainTracker.SettingsButton` is clicked: show `SettingsClosed`
4. When `SettingsClosed.*.OpenButton` is clicked: expand corresponding section

- 🤔 `ErrorMessage` should be at the bottom of the main window to always display error messages:
	- `⚠️ Config not found: initializing default config`
	- `⚠️ Unable to save config: {exception.message}`
	- `⚠️ Config updated: please start or restart Rocket League`
	- ...
