using System.Text.Json.Serialization;

namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadClockUpdatedSeconds(
	string? matchGuid,
	int? timeSeconds,
	bool? bOvertime) : IPayload
{
	public string? MatchGuid { get; } = matchGuid;
	public int? TimeSeconds { get; } = timeSeconds;

	[JsonPropertyName("bOvertime")]
	public bool? BOvertime { get; } = bOvertime;
}
