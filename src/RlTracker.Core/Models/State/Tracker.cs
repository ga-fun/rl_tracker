namespace RlTracker.Core.Models;

public sealed class Tracker
{
	public int Win { get; private set; }
	public int Loss { get; private set; }
	public int Streak { get; private set; }

	// Public

	public void AddResult(MatchResult result)
	{
		if (result == MatchResult.Win)
			AddWin();
		else
			AddLoss();
	}

	public void Reset()
	{
		Win = 0;
		Loss = 0;
		Streak = 0;
	}

	// Private

	private void AddWin()
	{
		Win++;
		Streak = Streak > 0 ? Streak + 1 : 1;
	}

	private void AddLoss()
	{
		Loss++;
		Streak = Streak < 0 ? Streak - 1 : -1;
	}
}
