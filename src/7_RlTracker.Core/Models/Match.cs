namespace GuillaumeAst.RlTracker.Core;

public sealed class Match(string guid, uint blueScore, uint orangeScore)
{
	public string Guid { get; } = guid;
	public uint BlueScore { get; set; } = blueScore;
	public uint OrangeScore { get; set; } = orangeScore;
}
