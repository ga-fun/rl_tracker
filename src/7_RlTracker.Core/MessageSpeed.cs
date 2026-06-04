using GuillaumeAst.Utils;

namespace GuillaumeAst.RlTracker.Core;

internal sealed class MessageSpeed
{
    private const long SpeedPrintDelaySec = 300;
    private long? _timeStartSec = null;
    private long? _timeLastSpeedPrint = null;
    private long _messageCount = 0;

	internal void Print()
	{
		_messageCount++;
		_timeStartSec ??= DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		_timeLastSpeedPrint ??= DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		long _timeCurrSec = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		
		if (_timeCurrSec - _timeLastSpeedPrint >= MessageSpeed.SpeedPrintDelaySec)
		{
			double messagePerSec = _messageCount / (double)(_timeCurrSec - _timeStartSec);
			Log.Write(Log.Level.Info, $"[{messagePerSec}/sec]");
			_timeLastSpeedPrint = _timeCurrSec;
		}
	}
}
