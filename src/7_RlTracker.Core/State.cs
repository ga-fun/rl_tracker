using GuillaumeAst.Utils;

namespace GuillaumeAst.RlTracker.Core;

public sealed class State : Notifier
{
	public Match? CurrentMatch
	{
		get;
		set;
	}
	public GameMode CurrentGameMode
	{
		get;
		private set
		{
			ValidateGameMode(value);
			if (field != value)
			{
				field = value;
				CurrentTracker = Trackers[(int)value];
				NotifyChange();
			}
		}
	}
	public IReadOnlyList<Tracker> Trackers { get; }
	public Tracker CurrentTracker
	{
		get;
		private set
		{
			if (field != value)
			{
				field = value;
				NotifyChange();
			}
		}
	}
	public Player? TrackedPlayer
	{
		get;
		set
		{
			if (field?.PrimaryId != value?.PrimaryId || field?.Platform != value?.Platform)
			{
				field = value;
				ResetTrackers();
				NotifyChange();
			}
		}
	}

	public State()
	{
		CurrentGameMode = GameMode.Other;
		Tracker[] trackers = new Tracker[(int)GameMode.Count];
		for (int index = 0; index < trackers.Length; index++)
		{
			trackers[index] = new();
		}
		Trackers = trackers;
		CurrentTracker = Trackers[(int)CurrentGameMode];
		TrackedPlayer = null;
	}

	public void PlusWin(GameMode gameMode)
	{
		CurrentGameMode = gameMode;
		Trackers[(int)gameMode].PlusWin();
	}

	public void MinusWin(GameMode gameMode)
	{
		CurrentGameMode = gameMode;
		Trackers[(int)gameMode].MinusWin();
	}

	public void PlusLoss(GameMode gameMode)
	{
		CurrentGameMode = gameMode;
		Trackers[(int)gameMode].PlusLoss();
	}

	public void MinusLoss(GameMode gameMode)
	{
		CurrentGameMode = gameMode;
		Trackers[(int)gameMode].MinusLoss();
	}

	public void ResetTracker(GameMode gameMode)
	{
		ValidateGameMode(gameMode);
		Trackers[(int)gameMode].Reset();
	}

	public void ResetTrackers()
	{
		foreach (Tracker tracker in Trackers)
		{
			tracker.Reset();
		}
	}

	private static void ValidateGameMode(GameMode gameMode)
	{
		if (gameMode < 0 || gameMode >= GameMode.Count)
		{
			throw new ArgumentOutOfRangeException(
				nameof(gameMode),
				gameMode,
				$"Invalid game mode {gameMode}: must be between 0 and {GameMode.Count - 1} (inclusive)");
		}
	}
}
