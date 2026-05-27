using System.Text.Json.Serialization;

namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadClockUpdatedSeconds(
	string? matchGuid,
	int? timeSeconds,
	bool? bOvertime) : StatsApiPayload
{
	public string? MatchGuid { get; } = matchGuid;
	public int? TimeSeconds { get; } = timeSeconds;

	[JsonPropertyName("bOvertime")]
	public bool? BOvertime { get; } = bOvertime;
}
