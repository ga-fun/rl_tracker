using GuillaumeAst.Utils;
using GuillaumeAst.RocketLeague.StatsApi;
using ApiPlayer = GuillaumeAst.RocketLeague.StatsApi.Player;
using ApiTeam = GuillaumeAst.RocketLeague.StatsApi.Team;

namespace GuillaumeAst.RlTracker.Core;

internal sealed class ApiEnventHandler(State state) : Notifier
{
	public sealed class Match
	{
		public string Guid { get; }
		public bool Ended
		{
			get;
			set
			{
				if (field != value)
				{
					field = value;
					Log.Write(Log.Level.Debug, $"Match.Ended = {field}");
				}
			}
		}
		public GameMode Mode
		{
			get;
			set
			{
				if (field != value)
				{
					field = value;
					Log.Write(Log.Level.Debug, $"Match.Mode = {field}");
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
					Log.Write(Log.Level.Debug, $"Match.WinnerSoFar = {field}");
				}
			}
		}

		public Match(string guid)
		{
			ArgumentNullException.ThrowIfNull(guid);
			Log.Write(Log.Level.Debug, "---");
			Log.Write(Log.Level.Debug, "Match created");
			Guid = guid;
			Log.Write(Log.Level.Debug, $"Match.Guid = {guid}");
			Mode = GameMode.Other;
			Log.Write(Log.Level.Debug, $"Match.Mode = {Mode}");
			WinnerSoFar = Team.None;
			Log.Write(Log.Level.Debug, $"Match.WinnerSoFar = {WinnerSoFar}");
			Ended = false;
			Log.Write(Log.Level.Debug, $"Match.Ended = {Ended}");
			Log.Write(Log.Level.Debug, "---");
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
					Log.Write(Log.Level.Info, $"Player.Name = {field}");
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
					Log.Write(Log.Level.Debug, $"Player.Id = {field}");
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
					Log.Write(Log.Level.Debug, $"Player.Shortcut = {field}");
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
					Log.Write(Log.Level.Info, $"Player.Team = {field}");
				}
			}
		}

		public Player(string name, string id)
		{
			Log.Write(Log.Level.Debug, "---");
			Log.Write(Log.Level.Debug, "Player created");
			Name = name;
			Id = id;
			Shortcut = null;
			Log.Write(Log.Level.Debug, $"Player.Shortcut = {Shortcut}");
			Team = null;
			Log.Write(Log.Level.Debug, $"Player.Team = {Team}");
			Log.Write(Log.Level.Debug, "---");
		}

		public void Reset()
		{
			Log.Write(Log.Level.Debug, "---");
			Log.Write(Log.Level.Debug, "Player.Reset()");
			Shortcut = null;
			Team = null;
			Log.Write(Log.Level.Debug, "---");
		}
	}

	private const string TrainingMatchGuid = "";
	private readonly State State = state;
	private readonly MessageSpeed _speed = new();
	private readonly Lock _gate = new();
	private Match CurrentMatch { get; set; } = new(TrainingMatchGuid){Ended = true};
	private Player? CurrentPlayer { get; set; }
	private bool InReplay
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				Log.Write(Log.Level.Debug, $"InReplay = {field}");
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
				Log.Write(Log.Level.Debug, "#############################################################");
				Log.Write(Log.Level.Debug, $"Handling event {field}");
			}
		}
	}

	internal void HandleEvent(Event apiEvent)
	{
		lock (_gate)
		{
			CurrentEventType = apiEvent.Type;
			string eventMatchGuid = apiEvent.Payload.MatchGuid;
			if (eventMatchGuid == TrainingMatchGuid)
			{
				StopCurrentMatch();
			}
			else if (CurrentEventType == EventType.RoundStarted && CurrentMatch.Guid != eventMatchGuid)
			{
				StopCurrentMatch();
				CurrentMatch = new(eventMatchGuid);
				InReplay = false;
			}
			else if (eventMatchGuid == CurrentMatch.Guid && CurrentMatch.Ended == true)
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
			else if (CurrentEventType == EventType.UpdateState)
			{
				UpdateState((PayloadUpdateState)apiEvent.Payload);
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

	private void UpdateState(PayloadUpdateState payload)
	{
		if (CurrentMatch.Guid != payload.MatchGuid)
		{
			// Avoid computing incoherent UpdateState sent between private match games
			return;
		}
		UpdateMode(payload);
		UpdatePlayer(payload);
		UpdateWinner(payload);
	}

	private void UpdateMode(PayloadUpdateState payload)
	{
		if (payload.Players.Count > 1)
		{
			int teamSize = (payload.Players.Count + 1) / 2;
			if (teamSize > (int)CurrentMatch.Mode && teamSize < (int)GameMode.Count)
			{
				CurrentMatch.Mode = (GameMode)teamSize;
			}
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
				Log.Write(Log.Level.Warning, $"Player [{payload.Game.Target?.Shortcut}] not found");
				return;
			}

			if (CurrentPlayer?.Id != apiPlayer.PrimaryId)
			{
				CurrentPlayer = new(apiPlayer.Name, apiPlayer.PrimaryId);
			}
			if (CurrentPlayer != null)
			{
				CurrentPlayer.Shortcut = apiPlayer.Shortcut;
				CurrentPlayer.Team = (Team)apiPlayer.TeamNum;
			}
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

	private void UpdateWinner(PayloadUpdateState payload)
	{

		if (payload.Game.BHasWinner == true)
		{
			CurrentMatch.WinnerSoFar = GetWinnerTeamFromWinnerOrScore(payload);
			StopCurrentMatch();
		}
		else
		{
			CurrentMatch.WinnerSoFar = GetWinnerTeamFromScore(payload);
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

	private void StopCurrentMatch()
	{
		if (CurrentMatch.Ended == true)
		{
			return;
		}
		else if (CurrentPlayer?.Team == null)
		{
			Log.Write(Log.Level.Warning, "Unable to compute match result because player or player's team is null");
			return;
		}
		if (CurrentPlayer.Team == CurrentMatch.WinnerSoFar)
		{
			State.PlusWin(CurrentMatch.Mode);
			Log.Write(Log.Level.Info, $"=> [{CurrentMatch.Mode} - WIN]");
		}
		else
		{
			State.PlusLoss(CurrentMatch.Mode);
			Log.Write(Log.Level.Info, $"=> [{CurrentMatch.Mode} - LOSS]");
		}
		CurrentMatch.Ended = true;
		CurrentPlayer?.Reset();
		InReplay = false;
	}
}
