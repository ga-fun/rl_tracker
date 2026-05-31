namespace RlTracker.Core.Models;

public sealed class Match(string guid, GameMode mode)
{
	public string Guid { get; } = guid;
	public GameMode Mode { get; } = mode;
	public uint OrangeScore { get; set; } = 0;
	public uint BlueScore { get; set; } = 0;
}
