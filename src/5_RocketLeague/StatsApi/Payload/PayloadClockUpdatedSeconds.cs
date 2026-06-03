using System.Text.Json.Serialization;

namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadClockUpdatedSeconds(
	string matchGuid,
	int timeSeconds,
	bool bOvertime) : Payload(matchGuid)
{
	public int TimeSeconds { get; } = timeSeconds;
	[JsonPropertyName("bOvertime")]
	public bool BOvertime { get; } = bOvertime;
}
