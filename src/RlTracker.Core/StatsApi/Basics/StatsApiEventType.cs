namespace RlTracker.Core.StatsApi;

internal enum StatsApiEventType
{
	UpdateState,
	BallHit,
	ClockUpdatedSeconds,
	CountdownBegin,
	CrossbarHit,
	GoalReplayEnd,
	GoalReplayStart,
	GoalReplayWillEnd,
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
