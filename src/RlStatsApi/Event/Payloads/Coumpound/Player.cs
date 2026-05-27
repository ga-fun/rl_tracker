using System.Text.Json.Serialization;

namespace RlStatsApi;

internal sealed class Player(
	string? name,
	string? primaryId,
	int? shortcut,
	int? teamNum,
	int? score,
	int? goals,
	int? shots,
	int? assists,
	int? saves,
	int? touches,
	int? carTouches,
	int? demos,
	bool? bHasCar,
	double? speed,
	int? boost,
	bool? bBoosting,
	bool? bOnGround,
	bool? bOnWall,
	bool? bPowersliding,
	bool? bDemolished,
	PlayerRef? attacker,
	bool? bSupersonic
)
{
	public string? Name { get; } = name;
	public string? PrimaryId { get; } = primaryId;
	public int? Shortcut { get; } = shortcut;
	public int? TeamNum { get; } = teamNum;
	public int? Score { get; } = score;
	public int? Goals { get; } = goals;
	public int? Shots { get; } = shots;
	public int? Assists { get; } = assists;
	public int? Saves { get; } = saves;
	public int? Touches { get; } = touches;
	public int? CarTouches { get; } = carTouches;
	public int? Demos { get; } = demos;

	[JsonPropertyName("bHasCar")]
	public bool? BHasCar { get; } = bHasCar;

	public double? Speed { get; } = speed;
	public int? Boost { get; } = boost;

	[JsonPropertyName("bBoosting")]
	public bool? BBoosting { get; } = bBoosting;

	[JsonPropertyName("bOnGround")]
	public bool? BOnGround { get; } = bOnGround;

	[JsonPropertyName("bOnWall")]
	public bool? BOnWall { get; } = bOnWall;

	[JsonPropertyName("bPowersliding")]
	public bool? BPowersliding { get; } = bPowersliding;

	[JsonPropertyName("bDemolished")]
	public bool? BDemolished { get; } = bDemolished;

	public PlayerRef? Attacker { get; } = attacker;

	[JsonPropertyName("bSupersonic")]
	public bool? BSupersonic { get; } = bSupersonic;
}
