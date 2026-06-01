namespace GuillaumeAst.RlTracker.Core.Models;

public sealed class Match
{
	public string Guid { get; }
	public GameMode Mode { get; }
	public uint OrangeScore { get; set; } = 0;
	public uint BlueScore { get; set; } = 0;

	public Match(string guid, GameMode mode)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(guid);
		if (mode < 0 || mode >= GameMode.Count)
		{
			throw new ArgumentOutOfRangeException(
				nameof(mode),
				mode,
				$"Invalid game mode {mode}: must be between 0 and {GameMode.Count - 1} (inclusive).");
		}
		Guid = guid;
		Mode = mode;
	}
}
