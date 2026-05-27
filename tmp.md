# Ignore replays

In `UpdateState` if `bReplay == true` => ignore it

# StatsApi

- `Event`:
	- Data:
		- `Type`
		- `Payload`
	- Methods:
		- `new(rawMessage)`

- `StatsApiConfig`
	- Data:
		- `public int Port`
		- `public double PacketSendRate`
	- Methods:
		- `new(int port, double packetSendRate)`
		- `apply(string RlInstallDir)` (creates temporary `FileIni` files to update Epic and Steam configs)

- `StatsApi`

> Le module `StatsApi` est responsable du parsing des messages reçus depuis l'API, (mais ce n'est pas lui qui écoute sur le port ?)
> Le module `StatsApi` est responsable de trouver (automatiquement si possible) / éditer les `.ini`
> Le module `StatsApi` se sert du module `FileIni` pour load / edit / save les fichiers
> On veut la même config sur `Epic` et `Steam`
> Le module `StatsApi` ne garde pas les settings en mémoire, il check juste qu'ils sont corrects dans les `.ini`

# Flow

1. Load `settings.json` pour charger les paramètre de `RlTracker`
2. Initialiser `StatsApi` selon les paramètres de `RlTracker`:
	- `RlTracker` récupère `Port`, `PacketSendRate`, `EpicRlDir` et `SteamRlDir` depuis `settings.json`
	- `RlTracker` appelle `StatsApi` pour qu'il check les `.ini` et les modifie si besoin:
		- `RlTracker` doit savoir si les fichiers ont été modifiés pour pouvoir dire au user de restart RL
3. `RlTracker` écoute les messages reçus par l'API:
	- `RlTracker` appelle `StatsApi` pour parser le message reçu (`Event(message)`)
	- `RlTracker` update son state interne en fonction des datas reçues