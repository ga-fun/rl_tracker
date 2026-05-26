#ifndef TRACKER_H
# define TRACKER_H

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

#endif
