# REFACTOR

## STRUCTURE ACTUELLE

```bash
.
└── src
    ├── FileIni
    │   ├── IniFile.cs
    │   ├── IniFile.Normalize.cs
    │   ├── IniFile.Read.cs
    │   └── IniFile.Write.cs
    ├── RlStatsApi
    │   ├── Config.cs
    │   ├── Event.cs
    │   └── Payloads
    │       ├── Basics
    │       │   ├── Ball.cs
    │       │   ├── PlayerRef.cs
    │       │   ├── Team.cs
    │       │   ├── Type.cs
    │       │   └── Vector.cs
    │       ├── Coumpound
    │       │   ├── BallHitBall.cs
    │       │   ├── BallLastTouch.cs
    │       │   ├── Game.cs
    │       │   └── Player.cs
    │       ├── Payload.cs
    │       ├── PayloadBallHit.cs
    │       ├── PayloadClockUpdatedSeconds.cs
    │       ├── PayloadCountdownBegin.cs
    │       ├── PayloadCrossbarHit.cs
    │       ├── PayloadGoalReplayEnd.cs
    │       ├── PayloadGoalReplayStart.cs
    │       ├── PayloadGoalReplayWillEnd.cs
    │       ├── PayloadGoalScored.cs
    │       ├── PayloadMatchCreated.cs
    │       ├── PayloadMatchDestroyed.cs
    │       ├── PayloadMatchEnded.cs
    │       ├── PayloadMatchInitialized.cs
    │       ├── PayloadMatchPaused.cs
    │       ├── PayloadMatchUnpaused.cs
    │       ├── PayloadPodiumStart.cs
    │       ├── PayloadReplayCreated.cs
    │       ├── PayloadRoundStarted.cs
    │       ├── PayloadStatfeedEvent.cs
    │       └── PayloadUpdateState.cs
    ├── RlTracker.Cli
    │   └── Program.cs
    └── RlTracker.Core
        ├── Config
        │   ├── Config.cs
        │   ├── ConfigGraphic.cs
        │   ├── RlInstall.cs
        │   ├── RlInstallEpic.cs
        │   └── RlInstallSteam.cs
        ├── Connection
        │   ├── Client.cs
        │   └── ConnectionManager.cs
        ├── Driver.cs
        ├── MessageHandler.cs
        ├── State
        │   ├── Models
        │   │   ├── GameMode.cs
        │   │   ├── Match.cs
        │   │   ├── Player.cs
        │   │   └── Tracker.cs
        │   └── State.cs
        └── Utils
            ├── Log.cs
            ├── Notifier.cs
            └── RlProcess.cs
```

## STRUCTURE FUTURE

> **PUB**: Expose public API
> **INT**: Expose internal API
> **P/I**: public class/getters + private constructor/setters

- Create `Utils`:
	- **PUB** `Log.cs`
	- **PUB** `Notifier.cs`
- Create `Filevdf`:
	- **PUB** `VdfFile.cs` (extract read logic from `RocketLeague.InstallSteam.cs`)
- Refactor `FileIni`:
	- **PUB** `IniFile.cs`
	- **INT** `Normalizer.cs`
	- **INT** `Reader.cs`
	- **INT** `Writer.cs`
- Create `Connection.cs`
	- **PUB** `Config.cs` (uri, retryDelay, callbacks (utiliser events plutôt ?!))
	- **PUB** `Manager.cs`
	- **INT** `Client.cs`
- Create `RocketLeague`:
	- **PUB** `Process.cs`
	- **PUB** `Config.cs` (possède `InstallSteam`, `InstallEpic` et `StatsApiConfig`)
	- `Install/`:
		- **P/I** `Install.cs` (abstract)
		- **P/I** `InstallSteam.cs`
		- **P/I** `InstallEpic.cs`
	- `StatsApi/`:
		- **P/I** `StatsConfig.cs`
		- **PUB** `StatsEvent.cs`
		- **PUB** `StatsPayload/**/*.cs`
- Create `RlTracker.Settings`:
	- **PUB** `UiConfig.cs`
	- **PUB** `GlobalConfig.cs` => `RocketLeague.Config` + `UiConfig`
	- **INT** `Serializer.cs`
- Create `RlTracker.Driver`:
	- **PUB** `Program.cs` => *ENTRY POINT TEMPORAIRE*
	- **PUB** `Driver.cs`  => *ORCHESTRATOR*
	- **INT** `MessageHandler.cs`
	- **PUB** `State/`:
		- **PUB** `State.cs`
		- **PUB** `Models/**/*.cs`
- Create `RlTracker.Ui`:
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
