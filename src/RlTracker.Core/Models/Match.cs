namespace RlTracker.Core.Models;

public sealed class Match(string guid)
{
	public string Guid { get; set; } = guid;
	public uint OrangeScore { get; set; } = 0;
	public uint BlueScore { get; set; } = 0;
}
