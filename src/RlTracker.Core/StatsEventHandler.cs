using RlStatsApi;

namespace RlTracker.Core;

// TODO: Print messages/sec treatment speed (every 10 sec)

internal sealed class StatsEventHandler
{
	// TODO
	internal void HandleEvent(Event apiEvent)
	{
		if (apiEvent.Type == RlStatsApi.Type.UpdateState)
		{
			// TODO
		}
		if (apiEvent.Type == RlStatsApi.Type.GoalScored)
		{
			// TODO
		}
		if (apiEvent.Type == RlStatsApi.Type.MatchEnded)
		{
			// TODO
		}
		if (apiEvent.Type == RlStatsApi.Type.MatchDestroyed)
		{
			// TODO
		}
	}
}
