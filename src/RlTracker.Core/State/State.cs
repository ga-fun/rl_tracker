namespace RlTracker.Core.Models;

public sealed class State : Notifier
{
	public enum ConnectionStatus
	{
		Disconnected,
		Connecting,
		Connected
	}

	public ConnectionStatus ClientStatus
	{
		get;
		set
		{
			field = value;
			NotifyChange();
		}
	}
	public GameMode CurrentGameMode
	{
		get;
		private set
		{
			field = value;
			NotifyChange();
		}
	}
	public Tracker[] Trackers { get; }
	public Tracker CurrentTracker
	{
		get;
		private set
		{
			field = value;
			NotifyChange();
		}
	}
	public Player? TrackedPlayer
	{
		get;
		set
		{
			if (field?.PrimaryId == value?.PrimaryId)
				return;
			field = value;
			ResetTrackers();
			NotifyChange();
		}
	}

	public State(ConnectionStatus clientSatus)
	{
		ClientStatus = clientSatus;
		CurrentGameMode = GameMode.Other;
		Trackers = new Tracker[(int)GameMode.Count];
		for (int index = 0; index < Trackers.Length; index++)
			Trackers[index] = new();
		CurrentTracker = Trackers[(int)CurrentGameMode];
		TrackedPlayer = null;
	}

	public void PlusWin(GameMode gameMode)
	{
		ValidateGameMode(gameMode);
		Trackers[(int)gameMode].PlusWin();
		UpdateCurrentTracker(gameMode);
	}

	public void MinusWin(GameMode gameMode)
	{
		ValidateGameMode(gameMode);
		Trackers[(int)gameMode].MinusWin();
		UpdateCurrentTracker(gameMode);
	}

	public void PlusLoss(GameMode gameMode)
	{
		ValidateGameMode(gameMode);
		Trackers[(int)gameMode].PlusLoss();
		UpdateCurrentTracker(gameMode);
	}

	public void MinusLoss(GameMode gameMode)
	{
		ValidateGameMode(gameMode);
		Trackers[(int)gameMode].MinusLoss();
		UpdateCurrentTracker(gameMode);
	}

	public void ResetTracker(GameMode gameMode)
	{
		ValidateGameMode(gameMode);
		Trackers[(int)gameMode].Reset();
	}

	public void ResetTrackers()
	{
		foreach (Tracker tracker in Trackers)
			tracker.Reset();
	}

	private void UpdateCurrentTracker(GameMode gameMode)
	{
		if (gameMode == CurrentGameMode)
			return;
		CurrentGameMode = gameMode;
		CurrentTracker = Trackers[(int)gameMode];
	}

	private static void ValidateGameMode(GameMode gameMode)
	{
		if (gameMode < 0 || gameMode >= GameMode.Count)
			throw new ArgumentOutOfRangeException($"Invalid game mode: {gameMode}");
	}
}
