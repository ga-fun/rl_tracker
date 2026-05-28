# Ignore replays

In `UpdateState` if `bReplay == true` => ignore it

# TODO

- `EpicRlInstallDir` and `SteamRlInstallDir` setters (reject if `IsRlInstallDir()` == false)

- `FindSteamRlInstallDir()` et `FindEpicRlInstallDir()` => **D'ABORD** chercher dans **LE** Path standard :
1. Path standard
2. From manifest
3. Fallback paths
