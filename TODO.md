# TODO

- `RlNotFound` + `RlNeedRestart` => move to `RocketLeague.Config`
- `Network.Client` => Handle `TCP Frames` ?

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

## Components

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


## Windows

- `MinimalWindow`:
	- `MainTracker`
	- `SettingsButton`
- `ConfigWindow`:
	- `UiSettings`
	- `InstallSettings`
	- `ClientSettings`

## Flow

1. If `RlNotFound == true` => only show `ConfigWindow`
2. Else => show `MinimalWindow`
3. When `SettingsButton` is clicked: show `SettingsWindow`

- Catch + error messages:
	- `⚠️ Config not found: initializing default config`
	- `⚠️ Unable to save config: {exception.message}`
	- `⚠️ Unable to find Rocket League (Epic version)`
	- `⚠️ Unable to find Rocket League (Steam version)`
	- `⚠️ Config updated: please start or restart Rocket League`
