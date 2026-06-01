# TEST

- ⚠️ `InstallSteam` et `InstallEpic` constructors:
	- If `installDir != null && isValid == false` => `AutoDetectInstallDir()`
- ⚠️ `Settings.Config` deserialization

# REFACTOR

- ⚠️ Dans les `setters` => remplacer :
	- `if (field == value) { return; }`
	- par `if (field != value) { ... }`

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

## STRUCTURE FUTURE

> **PUB**: Expose public API
> **INT**: Expose internal API
> **P/I**: public class/getters + private constructor/setters

- `Utils`:
	- **PUB** `Log.cs`
	- **PUB** `Notifier.cs`
- `Filevdf`:
	- **PUB** `VdfFile.cs` (extract read logic from `RocketLeague.InstallSteam.cs`)
- `FileIni`:
	- **PUB** `IniFile.cs`
	- **INT** `Normalizer.cs`
	- **INT** `Reader.cs`
	- **INT** `Writer.cs`
- `Connection`
	- **PUB** `Config.cs` (uri, retryDelay, callbacks (utiliser events plutôt ?!))
	- **PUB** `Manager.cs`
	- **INT** `Client.cs`
- `RocketLeague`:
	- **PUB** `Process.cs`
	- **PUB** `Config.cs` (possède `InstallSteam`, `InstallEpic` et `StatsApiConfig`)
	- `Install/`:
		- **P/I** `Install.cs` (abstract)
		- **P/I** `InstallSteam.cs`
		- **P/I** `InstallEpic.cs`
	- `StatsApi/`:
		- **P/I** `Config.cs`
		- **PUB** `Event.cs`
		- **PUB** `Payload/**/*.cs`
- `RlTracker.Settings`:
	- **PUB** `UiConfig.cs`
	- **PUB** `GlobalConfig.cs` => `RocketLeague.Config` + `UiConfig`
	- **INT** `Serializer.cs`
- `RlTracker.Driver`:
	- **PUB** `Program.cs` => *ENTRY POINT TEMPORAIRE*
	- **PUB** `Driver.cs`  => *ORCHESTRATOR*
	- **INT** `MessageHandler.cs`
	- **PUB** `State/`:
		- **PUB** `State.cs`
		- **PUB** `Models/**/*.cs`
- `RlTracker.Ui`:
	- **PUB** `Program.cs` => *ENTRY POINT FINAL*
	- *TODO...*

- Les projets "helpers" :
	- `Utils` propose du syntaxic sugar pour l'affichage de log et la propagation des property changed events
	- `FileVdf` sait lire des fichier .vdf
	- `FileIni` sait lire/modifier/écrire des fichier .ini
	- `Connection` sait gérer la connection loop avec un websocket et transmettre des messages complets
	- `RocketLeague` sait manipuler le jeu RL (trouver/vérifier où il est installé, modifier sa config interne, parser les messages de StatsApi...)
- `RlTracker.Settings` centralise les settings
	- `Ui.Config` gère la config de l'UI
	- `GlobalConfig` gère la création/modification de la config globale (`RocketLeague.Config` + `Ui.Config`)
	- `Serializer` gère la serialization/deserialization des settings
- `RlTracker.Driver` s'occupe de tout ce qui est la "vraie" logique du programme lui-même :
	- `Program` *ENTRY POINT TEMPORAIRE*
	- `Driver` orchestre le chargement de la config, du state, de la connection et dispatch les messages au handler
	- `Message Handler` parse les messages reçus depuis l'API stats et update `State`
	- `State` gère la représentation interne du state du programme
- `RlTracker.Ui` gère l'Ui
	- `Program` *ENTRY POINT FINAL* => invoque le `Driver`
	- *TODO...* => Affiche/update `Driver.State` et `Settings.GlobalConfig`

## Flow

- `RlTracker.Ui` **invoque** `RlTracker.Driver`
- `RlTracker.Driver` **load / initialise** `Settings.Config` et `RlTracker.Driver.State`
- `RlTracker.Ui` **affiche** `Settings.Config` et `RlTracker.Driver.State`
- `RlTracker.Ui` **update** `Settings.Config` et `RlTracker.Driver.State` (via input du user)
- `RlTracker.Driver` **update** `RlTracker.Driver.State` (via `RlTracker.MessageHandler`)

## Références

> `Log` est ignoré car c'est un module temporaire, à terme il n'y aura pas de connection avec le terminal

- `Utils` **référence**:
	- NONE
- `FileVdf` **référence**:
	- NONE
- `FileIni` **référence**:
	- NONE
- `Connection` **référence**:
	- NONE
- `RocketLeague` **référence**:
	- `Utils` (pour notify property changed de `RocketLeague.Config`, `RocketLeague.InstallSteam`, `RocketLeague.InstallEpic`)
	- `FileIni` (pour lire/écrire le .ini)
	- `FileVdf` (pour lire le .vdf)
- `Settings` **référence**:
	- `RocketLeague` (pour lire/edit `RocketLeague.Config`)
- `RlTracker.Driver` **référence**:
	- `Utils` (pour notify property changed de `RlTracker.Driver.State`)
	- `Connection` (pour connecter/déconnecter le websocket et recevoir les messages)
	- `RocketLeague` (pour lire `RocketLeague.Config`/`RocketLeague.Process` et pour parser les messages via `RocketLeague.StatsApi.Event`)
	- `Settings` (pour init/update `Settings.GlobalConfig` et le load/save via `Settings.Serializer`)
- `RlTracker.Ui` **référence**:
	- `Settings` (pour afficher `Settings.GlobalConfig`)
	- `RlTracker.Driver` (pour afficher `RlTracker.Driver.State` et update `RlTracker.Driver.State`/`Settings.GlobalConfig` via méthodes du Driver)

---

# TODO ?

- `ConnectionManager` => change callbacks for Actions ?
- `RlNotFound` should be a property of `RocketLeague.Config`
- Isolate `VdfFile` from `RocketLeague.InstallSteam`

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
