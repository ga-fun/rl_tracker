namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiTeam(
	string? name,
	int? teamNum,
	int? score,
	string? colorPrimary,
	string? colorSecondary
)
{
	public string? Name { get; } = name;
	public int? TeamNum { get; } = teamNum;
	public int? Score { get; } = score;
	public string? ColorPrimary { get; } = colorPrimary;
	public string? ColorSecondary { get; } = colorSecondary;
}
