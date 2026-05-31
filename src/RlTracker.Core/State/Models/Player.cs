namespace RlTracker.Core.Models;

public sealed class Player
{
	public enum PlatformType
	{
		Epic,
		Steam,
		PlayStation,
		Xbox,
		Switch,
		Unknown
	}

	public enum Team
	{
		Blue,
		Orange
	}

	public string Name { get; }
	public string PrimaryId { get; }
	public PlatformType Platform { get; }
	public string Id { get; }
	public Team? TeamColor { get; set; }
	public int? Shortcut { get; set; }

	public Player(string name, string primaryId)
	{
		ArgumentNullException.ThrowIfNull(name);
		ArgumentException.ThrowIfNullOrWhiteSpace(primaryId);

		string[] parts = primaryId.Split('|');
		if (parts.Length != 3)
			throw new FormatException($"Invalid primaryId format: \"{primaryId}\".");
		if (!Enum.TryParse(parts[0], ignoreCase: true, out PlatformType platform))
			throw new FormatException($"Unknown platform: \"{parts[0]}\".");
		
		Name = name;
		PrimaryId = primaryId;
		Platform = platform;
		Id = parts[1];
	}
}
