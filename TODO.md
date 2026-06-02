# TODO

- `RlNotFound` + `RlNeedRestart` => move to `RocketLeague.Config`
- `Network.Client` => Handle `TCP Frames` ?

## FINAL
- `RlTracker.Ui.csproj` change `<OutputType>Exe</OutputType>` to `<OutputType>WinExe</OutputType>`

---

# NORME

- Namespaces `GuillaumeAst.`:
```cs
// Au lieu de
namespace MyNameSpace;
// Plutôt faire
namespace GuillaumeAst.MyNameSpace;
```

- Ne pas `throw` si le call system le fait déjà:
```cs
// Au lieu de
if (!File.Exists(file))
{
	throw new FileNotFoundException(...);
}
string content = File.ReadAllText(file);
// Plutôt faire
string content = File.ReadAllText(file);
```

- Accolades sur les `if`, les boucles etc:
```cs
// Au lieu de 
if (true)
	DoThis();
// Plutôt faire
if (true)
{
	DoThis();
}
```

- Ne **JAMAIS** faire de catch **global**:
```cs
// Ne pas faire
try
{...}
catch
{...}
// Ni
try
{...}
catch (Exception exception)
{...}
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
