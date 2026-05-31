# JMA

Hello, encore bravo pour le match ! 🏆

De mon côté j'ai l'impression de commencer à y voir plus clair sur comment "bien" coder en *OOP*.
J'ai essayé d'utiliser les *inner class* et l'*héritage* pour mieux organiser mon code.

Je te file donc le repo pour que tu puisses y jeter un oeil quand t'as le temps :
https://github.com/guillaumeast/rl_tracker

C'est encore *WIP* donc tout n'est pas implémenté (il manque nottamment l'`UI`, le `MessageHandler` et la gestion du `State` dans le `Driver`), mais je pense que ça suffit pour que tu puisses voir si je prends une mauvaise direction 🙈.

Selon moi, en allant du "best" au "worst" module :
- 😍 `src/RlTracker.Core/Connection/*.cs`
- 😍 `src/RlTracker.Core/State/**/*.cs`
- 👌 `src/RlTracker.Core/Driver.cs`
- 🤔 `src/RlTracker.Core/Utils/**/*.cs` (debug + **tous petits fichiers** sans lien fort avec les autre modules)
- 🤔 `src/RlStatsApi/**/*.cs` (**plein de fichiers** pour parser les messages de l'API du jeu)
- 🤮 `src/FileIni/*.cs` (une **énorme classe de 4 fichiers** pour gérer les fichiers `.ini`)

`src/RlStatsApi/Config/**/*.cs`

Dis-moi si je me trompe mais de ce que j'ai compris il faudrait que :

1. Je **SPLIT** `src/FileIni/*.cs` en :
- `FinelIniReader` (fonction `Read()` + helpers)
- `FileIniWriter` (fonction `Write()` + helpers)
- `FileIniNormalizer` (fonctions `Normalize*()`)
et que je mette ces classes en `internal` pour qu'elles ne soient pas visibles en dehors du *projet* `src/FileIni`

2. Je **MERGE** ce qui touche à *Rocket League* dans un *projet* `src/RocketLeague`:
- `src/RlTracker.Core/Config/RlInstall*.cs`
- `src/RlTracker.Core/Utils/RlProcess.cs`
- `src/RlStatsApi/**/*.cs`

PS: J'ai pas pris le temps d'installer `SonarCube` (comme tout bon truc de dev ça ne fonctionne pas "out of the box" faut faire tout un tas de manip que j'ai un peu la flemme de faire pour l'instant 🙈)

😘

## Call

1. Enums / Inner class
2. Attributs statiques
3. Attributs public
4. Attributs private
5. Constructor

