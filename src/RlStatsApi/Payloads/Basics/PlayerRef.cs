namespace RlStatsApi;

internal sealed class PlayerRef(string? name, int? shortcut, int? teamNum)
{
	public string? Name { get; } = name;
	public int? Shortcut { get; } = shortcut;
	public int? TeamNum { get; } = teamNum;
}
