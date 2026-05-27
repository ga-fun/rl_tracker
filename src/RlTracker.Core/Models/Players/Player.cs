namespace RlTracker.Core.Models;

public sealed class Player
{
	public string Name { get; private set; }
	/// <summary>
	/// Raw player id received from Rocket League API.
	/// </summary>
	public string? PrimaryId { get; private set; }
	public Platform? Platform { get; private set; }
	public string? Id { get; private set; }
	public Team? Team { get; set; }

	public Player(string name)
	{
		ArgumentNullException.ThrowIfNull(name);
		Name = name;
	}
	// TODO: setters (after RocketLeague API implementation)
}
