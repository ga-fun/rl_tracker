using GuillaumeAst.Utils;
using GuillaumeAst.RocketLeague.StatsApi;
using ApiPlayer = GuillaumeAst.RocketLeague.StatsApi.Player;
using ApiTeam = GuillaumeAst.RocketLeague.StatsApi.Team;

namespace GuillaumeAst.RlTracker.Core;

internal sealed class ApiEnventHandler(State state)
{
	public enum MatchStatus
	{
		Pending,
		InProgress,
		Ended
	}

	public sealed class Match
	{
		public string Guid { get; }
		public GameMode Mode
		{
			get;
			set
			{
				if (field != value)
				{
					field = value;
					Log.Print($"Match.Mode = {Log.Yellow}{field}");
				}
			}
		}
		public Team WinnerSoFar
		{
			get;
			set
			{
				if (field != value)
				{
					field = value;
					Log.Write($"Match.WinnerSoFar = {field}");
				}
			}
		}
		public MatchStatus Status
		{
			get;
			set
			{
				if (field != value)
				{
					field = value;
					Log.Print($"Match.Status = {Log.Yellow}{field}");
				}
			}
		}

		public Match(string guid)
		{
			ArgumentNullException.ThrowIfNull(guid);
			Log.Write("---");
			Log.Print("Match created");
			Guid = guid;
			Log.Write($"Match.Guid = {guid}");
			Mode = GameMode.Other;
			Log.Write($"Match.Mode = {Mode}");
			WinnerSoFar = Team.None;
			Log.Write($"Match.WinnerSoFar = {WinnerSoFar}");
			Status = MatchStatus.Pending;
			Log.Write($"Match.Status = {Status}");
			Log.Write("---");
		}
	}

	private sealed class Player
	{
		public string Name
		{
			get;
			set
			{
				if (field != value)
				{
					field = value;
					Log.Print($"Player.Name = {Log.Yellow}{field}");
				}
			}
		}
		public string Id
		{
			get;
			set
			{
				if (field != value)
				{
					field = value;
					Log.Write($"Player.Id = {field}");
				}
			}
		}
		public int? Shortcut
		{
			get;
			set
			{
				if (field != value)
				{
					field = value;
					Log.Write($"Player.Shortcut = {field}");
				}
			}
		}
		public Team? Team
		{
			get;
			set
			{
				if (field != value)
				{
					field = value;
					Log.Print($"Player.Team = {Log.Yellow}{field}");
				}
			}
		}

		public Player(string name, string id)
		{
			Log.Write("---");
			Log.Write("Player created");
			Name = name;
			Id = id;
			Shortcut = null;
			Log.Write($"Player.Shortcut = {Shortcut}");
			Team = null;
			Log.Write($"Player.Team = {Team}");
			Log.Write("---");
		}

		public void Reset()
		{
			Log.Write("---");
			Log.Write("Player.Reset()");
			Shortcut = null;
			Team = null;
			Log.Write("---");
		}
	}

	private const int MatchDurationSec = 300;
	private const string TrainingMatchGuid = "";
	private readonly State State = state;
	private readonly MessageSpeed _speed = new();
	private readonly Lock _gate = new();
	private Match CurrentMatch { get; set; } = new(TrainingMatchGuid){Status = MatchStatus.Ended};
	private Player? CurrentPlayer { get; set; }
	private bool InReplay
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				Log.Write($"InReplay = {field}");
			}
		}
	} = false;
	private static EventType? CurrentEventType
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				Log.Write("#############################################################");
				Log.Write($"Handling event {field}");
			}
		}
	}

	internal void HandleEvent(Event apiEvent)
	{
		lock (_gate)
		{
			CurrentEventType = apiEvent.Type;
			if (apiEvent.Payload.MatchGuid == TrainingMatchGuid)
			{
				StopCurrentMatch();
			}
			else if (apiEvent.Payload.MatchGuid == CurrentMatch.Guid && CurrentMatch.Status == MatchStatus.Ended)
			{
				_speed.Print();
				return;
			}
			else if (CurrentEventType == EventType.ReplayPlaybackStart)
			{
				InReplay = true;
			}
			else if (CurrentEventType == EventType.ReplayPlaybackEnd)
			{
				InReplay = false;
			}
			else if (CurrentEventType == EventType.MatchInitialized)
			{
				NewMatch(((PayloadMatchInitialized)apiEvent.Payload).MatchGuid);
			}
			else if (CurrentEventType == EventType.RoundStarted && CurrentMatch.Status == MatchStatus.Pending)
			{
				NewMatch(((PayloadRoundStarted)apiEvent.Payload).MatchGuid);
				CurrentMatch.Status = MatchStatus.InProgress;
			}
			else if (CurrentEventType == EventType.UpdateState)
			{
				UpdateState((PayloadUpdateState)apiEvent.Payload);
			}
			else if (CurrentEventType == EventType.GoalScored && InReplay == false)
			{
				GoalScored((PayloadGoalScored)apiEvent.Payload);
			}
			else if (CurrentEventType == EventType.MatchEnded)
			{
				CurrentMatch.WinnerSoFar = (Team)((PayloadMatchEnded)apiEvent.Payload).WinnerTeamNum;
				StopCurrentMatch();
			}
			else if (CurrentEventType == EventType.MatchDestroyed)
			{
				StopCurrentMatch();
			}
			_speed.Print();
		}
	}

	private void NewMatch(string matchGuid)
	{
		if (string.IsNullOrWhiteSpace(matchGuid))
		{
			if (matchGuid == null)
				Log.PrintRed($"Training Guid = [null]");
			else if (string.IsNullOrEmpty(matchGuid))
				Log.PrintRed($"Training Guid = \"\"");
			// During training, events are sent with MatchGuid == "" => skip it
			return;
		}
		if (CurrentMatch.Guid != matchGuid)
		{
			StopCurrentMatch();
			CurrentMatch = new(matchGuid);
			InReplay = false;
		}
	}

	private void UpdateState(PayloadUpdateState payload)
	{
		UpdateMatch(payload);
		UpdateMode(payload);
		UpdatePlayer(payload);
		if (payload.Game.BHasWinner == true)
		{
			StopCurrentMatch();
		}
	}

	private void UpdateMatch(PayloadUpdateState payload)
	{
		if (CurrentMatch.Guid != payload.MatchGuid)
		{
			NewMatch(payload.MatchGuid);
		}
		if (payload.Game.BHasWinner == true)
		{
			CurrentMatch.WinnerSoFar = GetWinnerTeamFromWinnerOrScore(payload);
			CurrentMatch.Status = MatchStatus.Ended;
		}
		else
		{
			CurrentMatch.WinnerSoFar = GetWinnerTeamFromScore(payload);
			if (CurrentMatch.Status == MatchStatus.Pending && payload.Game.TimeSeconds < MatchDurationSec)
			{
				CurrentMatch.Status = MatchStatus.InProgress;
			}
		}
	}

	private static Team GetWinnerTeamFromWinnerOrScore(PayloadUpdateState payload)
	{
		foreach (ApiTeam team in payload.Game.Teams)
		{
			if (team.Name == payload.Game.Winner)
				return (Team)team.TeamNum;
		}
		return GetWinnerTeamFromScore(payload);
	}

	private static Team GetWinnerTeamFromScore(PayloadUpdateState payload)
	{
		int blueScore = payload.Game.Teams[(int)Team.Blue].Score;
		int orangeScore = payload.Game.Teams[(int)Team.Orange].Score;

		if (blueScore > orangeScore)
		{
			return Team.Blue;
		}
		else if (orangeScore > blueScore)
		{
			return Team.Orange;
		}
		else
		{
			return Team.None;
		}
	}

	private void UpdateMode(PayloadUpdateState payload)
	{
		if (payload.Players.Count == 1)
		{
			return;
		}
		int teamSize = (payload.Players.Count + 1) / 2;
		if (teamSize > (int)CurrentMatch.Mode && teamSize < (int)GameMode.Count)
		{
			CurrentMatch.Mode = (GameMode)teamSize;
		}
	}

	private void UpdatePlayer(PayloadUpdateState payload)
	{
		if (payload.Game.BReplay == false && payload.Game.Target != null
			&& CurrentPlayer?.Shortcut != payload.Game.Target.Shortcut)
		{
			ApiPlayer? apiPlayer = GetPlayerFromShortcut(payload.Players, payload.Game.Target.Shortcut);
			if (apiPlayer == null)
			{
				Log.Print($"{Log.Red}Player [{payload.Game.Target?.Shortcut}] not found");
				return;
			}

			if (CurrentPlayer == null || CurrentPlayer.Id != apiPlayer.PrimaryId)
			{
				CurrentPlayer = new(apiPlayer.Name, apiPlayer.PrimaryId);
			}
			CurrentPlayer.Shortcut = apiPlayer.Shortcut;
			CurrentPlayer.Team = (Team)apiPlayer.TeamNum;
		}
	}

	private static ApiPlayer? GetPlayerFromShortcut(IReadOnlyList<ApiPlayer> playerList, int shortcut)
	{
		foreach (ApiPlayer player in playerList)
		{
			if (player.Shortcut == shortcut)
				return player;
		}
		return null;
	}

	private static void GoalScored(PayloadGoalScored payload)
	{
		double startSpeed = Math.Round(payload.BallLastTouch.Speed);
		double goalSpeed = Math.Round(payload.GoalSpeed);

		Log.Print($"Goal scored: {startSpeed} km/h -> {goalSpeed} km/h");
	}

	private void StopCurrentMatch()
	{
		if (CurrentMatch.Status == MatchStatus.Ended)
		{
			return;
		}
		else if (CurrentMatch.Status == MatchStatus.Pending)
		{
			Log.Print("Match not started ignored");
		}
		else if (CurrentMatch.Status == MatchStatus.InProgress)
		{
			if (CurrentPlayer?.Team == null)
			{
				Log.Print($"{Log.Red}Unable to compute match result because player or player's team is null");
			}
			else
			{
				string Mode = "???";
				if (CurrentMatch.Mode == GameMode.OneVersusOne)
				{
					Mode = "1v1";
				}
				else if (CurrentMatch.Mode == GameMode.TwoVersusTwo)
				{
					Mode = "2v2";
				}
				else if (CurrentMatch.Mode == GameMode.ThreeVersusThree)
				{
					Mode = "3v3";
				}
				if (CurrentPlayer.Team == CurrentMatch.WinnerSoFar)
				{
					State.PlusWin(CurrentMatch.Mode);
					Log.Print($"{Log.Green}=> [{Mode}] WIN!");
				}
				else
				{
					State.PlusLoss(CurrentMatch.Mode);
					Log.Print($"{Log.Red}=> [{Mode}] LOSS");
				}
				Log.Print($"{Log.Blue}-------------");
				Log.Print($"{Log.Blue} {State.CurrentTracker.Win} | {State.CurrentTracker.Loss} | {State.CurrentTracker.Streak}");
				Log.Print($"{Log.Blue}-------------");
			}
		}
		CurrentMatch.Status = MatchStatus.Ended;
		CurrentPlayer?.Reset();
		InReplay = false;
	}
}
