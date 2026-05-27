using System.Text.Json.Serialization;

namespace RlStatsApi;

internal sealed class PayloadClockUpdatedSeconds(
	string? matchGuid,
	int? timeSeconds,
	bool? bOvertime) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
	public int? TimeSeconds { get; } = timeSeconds;

	[JsonPropertyName("bOvertime")]
	public bool? BOvertime { get; } = bOvertime;
}
