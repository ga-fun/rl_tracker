#include "tracker.h"
#include <stdlib.h>

/* ---------- MODE ---------- */

static void	mode_init(t_mode *mode)
{
	*mode = MODE_COUNT;
}

static void	mode_free(t_mode *mode)
{
	mode_init(mode);
}

/* ---------- TEAM ---------- */

static void	team_init(t_team *team)
{
	*team = TEAM_COUNT;
}

static void	team_free(t_team *team)
{
	team_init(team);
}

/* ---------- TRACKER ---------- */

static void	tracker_init(t_tracker *tracker)
{
	tracker->win = 0;
	tracker->loss = 0;
	tracker->streak = 0;
}

static void	tracker_free(t_tracker *tracker)
{
	tracker_init(tracker);
}

/* ---------- MATCH ---------- */

static void	match_init(t_match *match)
{
	match->guid = NULL;
	match->score = 0;
}

static void match_free(t_match *match)
{
	if (match->guid)
		free(match->guid);
	match_init(match);
}

/* ---------- PLAYER ---------- */

static void	player_init(t_player *player)
{
	player->name = NULL;
	player->id = NULL;
	team_init(&player->team);
}

static void	player_free(t_player *player)
{
	if (player->name)
		free(player->name);
	if (player->id)
		free(player->id);
	team_free(&player->team);
	player_init(player);
}

/* ---------- STATE ---------- */

void	state_init(t_state *state)
{
	mode_init(&state->mode);
	for (t_mode mode = 0; mode < MODE_COUNT; mode++)
		tracker_init(&state->trackers[mode]);
	match_init(&state->match);
	player_init(&state->player);
}

void	state_free(t_state *state)
{
	mode_free(&state->mode);
	for (t_mode mode = 0; mode < MODE_COUNT; mode++)
		tracker_free(&state->trackers[mode]);
	match_free(&state->match);
	player_free(&state->player);
}
