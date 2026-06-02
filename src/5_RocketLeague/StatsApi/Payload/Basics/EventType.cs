namespace GuillaumeAst.RocketLeague.StatsApi;

public enum EventType
{
	UpdateState,
	BallHit,
	ClockUpdatedSeconds,
	CountdownBegin,
	CrossbarHit,
	ReplayPlaybackEnd,		// GoalReplayEnd
	ReplayPlaybackStart,	// GoalReplayStart
	ReplayWillEnd,			// GoalReplayWillEnd
	GoalScored,
	MatchCreated,
	MatchInitialized,
	MatchDestroyed,
	MatchEnded,
	MatchPaused,
	MatchUnpaused,
	PodiumStart,
	ReplayCreated,
	RoundStarted,
	StatfeedEvent
}
