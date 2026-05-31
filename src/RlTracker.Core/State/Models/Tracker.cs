namespace RlTracker.Core.Models;

public sealed class Tracker : Notifier
{
	public uint Win
	{
		get;
		private set
		{
			field = value;
			NotifyChange();
		}
	} = 0;
	public uint Loss
	{
		get;
		private set
		{
			field = value;
			NotifyChange();
		}
	} = 0;
	public int Streak
	{
		get;
		private set
		{
			field = value;
			NotifyChange();
		}
	} = 0;

	public void PlusWin()
	{
		Win++;
		Streak = Streak > 0 ? Streak + 1 : 1;
	}

	public void MinusWin()
	{
		if (Win == 0)
			return;
		Win--;
		if (Streak > 0)
			Streak--;
	}

	public void PlusLoss()
	{
		Loss++;
		Streak = Streak < 0 ? Streak - 1 : -1;
	}

	public void MinusLoss()
	{
		if (Loss == 0)
			return;
		Loss--;
		if (Streak < 0)
			Streak++;
	}

	public void Reset()
	{
		Win = 0;
		Loss = 0;
		Streak = 0;
	}
}
