namespace RlTracker.Core.Models;

public sealed class Player(string name)
{
	public string Name { get; private set; } = name;
	/// <summary>
	/// Raw player id received from Rocket League API.
	/// </summary>
	public string? PrimaryId {
		get;
		set
		{
			// TODO
		}
	}
	public Platform? Platform { get; private set; }
	public string? Id { get; private set; }
	public Team? Team { get; set; }
	public int? Shortcut { get; set; }

	// TODO: setters (after RocketLeague API implementation)
}
