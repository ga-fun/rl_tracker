using RlStatsApi;

namespace RlTracker.Core;

// `UpdateState` => if `bReplay == true` => ignore it

internal sealed class MessageHandler
{
	private const long SpeedPrintDelaySec = 10;
	public double MessagePerSec { get; private set; } = 0;
	private long? _timeStartSec = null;
	private long? _timeCurrSec = null;
	private long _messageCount = 0;
	private long _timeLastSpeedPrint = 0;

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
		UpdateSpeed();
	}

	private void UpdateSpeed()
	{
		if (_timeStartSec == null)
			_timeStartSec = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		_timeCurrSec = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		_messageCount++;
		MessagePerSec = _messageCount / (double)(_timeCurrSec - _timeStartSec);
		if (_timeCurrSec - _timeLastSpeedPrint >= 10)
		{
			Log.PrintBlue($"===> [{MessagePerSec}/sec] <===");
			_timeLastSpeedPrint = _timeCurrSec.Value;
		}
	}
}
