using System.Text.Json.Serialization;

namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiGame(
	List<StatsApiTeam>? teams,
	int? timeSeconds,
	bool? bOvertime,
	int? frame,
	double? elapsed,
	StatsApiBall? ball,
	bool? bReplay,
	bool? bHasWinner,
	string? winner,
	string? arena,
	bool? bHasTarget,
	StatsApiPlayerRef? target
)
{
	public List<StatsApiTeam>? Teams { get; } = teams;
	public int? TimeSeconds { get; } = timeSeconds;

	[JsonPropertyName("bOvertime")]
	public bool? BOvertime { get; } = bOvertime;

	public int? Frame { get; } = frame;
	public double? Elapsed { get; } = elapsed;
	public StatsApiBall? Ball { get; } = ball;

	[JsonPropertyName("bReplay")]
	public bool? BReplay { get; } = bReplay;

	[JsonPropertyName("bHasWinner")]
	public bool? BHasWinner { get; } = bHasWinner;

	public string? Winner { get; } = winner;
	public string? Arena { get; } = arena;

	[JsonPropertyName("bHasTarget")]
	public bool? BHasTarget { get; } = bHasTarget;

	public StatsApiPlayerRef? Target { get; } = target;
}
