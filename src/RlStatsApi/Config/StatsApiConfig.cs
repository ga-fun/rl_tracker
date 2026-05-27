using RlTracker.Core.Config;

namespace RlStatsApi;

internal sealed class StatsApiConfig
{
	public int Port
	{
		get;
		set
		{
			if (value <= 0 || value > 65535)
				throw new ArgumentOutOfRangeException(
					nameof(Port),
					value,
					"Port must be between 1 and 65535."
				);
			field = value;
		}
	} = 49123;

	public double PacketSendRate
	{
		get;
		set
		{
			if (value <= 0)
				field = 1;
			else if (value > 120)
				field = 120;
			field = value;
		}
	} = 120;

	public void Apply(string rlInstallDir)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(rlInstallDir);
		
		FileIni
	}
}
