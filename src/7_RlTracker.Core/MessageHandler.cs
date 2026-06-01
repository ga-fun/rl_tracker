using GuillaumeAst.Utils;
using GuillaumeAst.RocketLeague.StatsApi;

namespace GuillaumeAst.RlTracker.Core;

// `UpdateState` => if `bReplay == true` => ignore it

internal sealed class MessageHandler
{
	private const long SpeedPrintDelaySec = 10;
	public double MessagePerSec { get; private set; } = 0;
	private long? _timeStartSec = null;
	private long _messageCount = 0;
	private long _timeLastSpeedPrint = 0;

	// TODO
	internal void HandleEvent(Event apiEvent)
	{
		if (apiEvent.Type == EventType.UpdateState)
		{
			// TODO
		}
		if (apiEvent.Type == EventType.GoalScored)
		{
			// TODO
		}
		if (apiEvent.Type == EventType.MatchEnded)
		{
			// TODO
		}
		if (apiEvent.Type == EventType.MatchDestroyed)
		{
			// TODO
		}
		UpdateSpeed();
	}

	private void UpdateSpeed()
	{
		_timeStartSec ??= DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		long _timeCurrSec = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		_messageCount++;
		MessagePerSec = _messageCount / (double)(_timeCurrSec - _timeStartSec);
		if (_timeCurrSec - _timeLastSpeedPrint >= 10)
		{
			Log.PrintBlue($"===> [{MessagePerSec}/sec] <===");
			_timeLastSpeedPrint = _timeCurrSec;
		}
	}
}
