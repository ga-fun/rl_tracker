namespace RlTracker.Core.Models;

// TODO:
// -> Tracked Player
// -> Win
// -> Loss
// -> Streak
// -> CurrentMatch
// -> CurrentScorez

internal sealed class State
{
	public ConnectionStatus ConnectionStatus { get; set; }
	public Tracker[GameMode.Count] Trackers { get; set; } = new();
	public Player? TrackedPlayer { get; set; }
	public Match? Match { get; set; }
}
