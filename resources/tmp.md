# DATAS

> Je mets ça en C car c'est plus simple pour moi mais ça ne veut pas dire qu'on les implémentera forcément en C

```c
typedef enum e_mode
{
	MODE_1V1,
	MODE_2V2,
	MODE_3V3,
	MODE_OTHER,
	MODE_COUNT
}	t_mode;

typedef enum e_team
{
	TEAM_BLUE,
	TEAM_ORANGE,
	TEAM_COUNT
}	t_team;

typedef struct s_tracker
{
	int	win;
	int	loss;
	int	streak;
}	t_tracker;

typedef t_tracker	t_trackers[MODE_COUNT];

typedef struct s_match
{
	char	*guid;
	int		score;
}	t_match;

typedef struct s_player
{
	char	*name;
	char	*id;
	t_team	team;
}	t_player;

typedef struct s_state
{
	t_mode		mode;
	t_trackers	trackers;
	t_match		match;
	t_player	player;
}	t_state;
```

`settings.json` doit contenir :
- `Steam`				(true/false, default = true)
- `SteamPath`			(default = "")	// TODO
- `Epic`				(true/false, default = true)
- `EpicPath`			(default = "")	// TODO
- `Port`				(default = 49123)
- `PacketSendRate`		(default = 120)
- `WinSYmbol`			(default = `✅`)
- `LoseSYmbol`			(default = `❌`)
- `WinStreakSYmbol`		(default = `🚀`)
- `LoseStreakSYmbol`	(default = `⚰️`)

# FLOW ENVISAGÉ

## INITIALISATION

1. Si `settings.json` n'existe pas => Le créer avec des valeurs par défaut
2. Vérifier que les fichiers `.ini` sont correctement initialisés
- Si incorrect => Initialiser `PacketSendRate` et `Port` + afficher un message "Config initialized. Please start or restart Rocket League." en orange en-dessous de l'input correspondant dans la section Config de l'UI
3. Initialisation de tous les `trackers` à 0 (un `tracker` pour chaque `mode` : 1v1/2v2/3v3/unknwon)
4. Listening automatique (donc pas de bouton `Start listening` ni `Stop`)

## GAME LOOP

1. Event `MatchCreated` :
- Stockage de `event.MatchGuid` dans `state.match.guid`
- Compute du `state.mode` (selon le nombre de joueurs dans les équipes)
2. Event `UpdateState` :
- Stockage de Détection du joueur (cf `target`) et stockage de son `Name` + `PrimaryId` + `TeamNum` dans `player`
- Si le `PrimaryId` a changé => Réinitialiser tous les `trackers`
3. Chaque event `GoalScored` :
- Update `match.score` (incrémenter si `GoalScored.TeamNum` == `player.TeamNum`, sinon décrémenter)
4. Event `MatchEnded` :
- Update le tracker selon `WinnerTeamNum` et `player.TeamNum`
- reset le `match` à 0
5. Event `MatchDestroyed` (uniquement si match.guid != NULL) :
- Update le tracker selon `match.score`

# QUESTIONS

1. Est-ce qu'on peut faire des sections qui peuvent s'ouvrir / se fermer avec `Windows Forms` ?
2. Est-ce qu'on peut afficher des emojis comme `🔥` ou `👎` dans `Windows Forms` ?
3. Est-ce que le champs `Target` de l'event `UpdateState` peut nous permettre de connaitre automatiquement le joueur de l'utilisateur (j'imagine que le joueur de l'utilisateur est celui de `Target` lors de la game qu'il joue ?!) ?
4. Etant donné que je maitrise le C, est-ce pertinent d'envisager une implémentation C ?
5. Est-ce que la structuration des données te parait pertinente ?
6. Est-ce que le flow logique te parait pertinent ?
7. Est-ce que l'UI envisagée (cf mockup ci-joint) te parait pertinente ?
