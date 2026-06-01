using System.Text.Json.Serialization;

namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class Game(
	IReadOnlyList<Team>? teams,
	int? timeSeconds,
	bool? bOvertime,
	int? frame,
	double? elapsed,
	Ball? ball,
	bool? bReplay,
	bool? bHasWinner,
	string? winner,
	string? arena,
	bool? bHasTarget,
	PlayerRef? target
)
{
	public IReadOnlyList<Team>? Teams { get; } = teams;
	public int? TimeSeconds { get; } = timeSeconds;

	[JsonPropertyName("bOvertime")]
	public bool? BOvertime { get; } = bOvertime;

	public int? Frame { get; } = frame;
	public double? Elapsed { get; } = elapsed;
	public Ball? Ball { get; } = ball;

	[JsonPropertyName("bReplay")]
	public bool? BReplay { get; } = bReplay;

	[JsonPropertyName("bHasWinner")]
	public bool? BHasWinner { get; } = bHasWinner;

	public string? Winner { get; } = winner;
	public string? Arena { get; } = arena;

	[JsonPropertyName("bHasTarget")]
	public bool? BHasTarget { get; } = bHasTarget;

	public PlayerRef? Target { get; } = target;
}
