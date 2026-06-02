using GuillaumeAst.Utils;
using GuillaumeAst.RocketLeague.StatsApi;
using ApiPlayer = GuillaumeAst.RocketLeague.StatsApi.Player;
using ApiTeam = GuillaumeAst.RocketLeague.StatsApi.Team;

namespace GuillaumeAst.RlTracker.Core;

internal sealed class MessageHandler(State state)
{
	private sealed class MessageSpeed
	{
		public const long SpeedPrintDelaySec = 300;
		public long? TimeStartSec = null;
		public long? TimeLastSpeedPrint = null;
		public long MessageCount = 0;
	}

	public sealed class Match(string guid)
	{
		public string Guid = string.IsNullOrWhiteSpace(guid)
			? throw new ArgumentException("Match guid must not be null or white-space.", nameof(guid))
			: guid;
		public GameMode Mode = GameMode.Other;
		public Team? WinnerSoFar;
		public bool HasSTarted
		{
			get;
			set
			{
				if (field ==false && value == true)
				{
					Log.Print("Match has started");
					field = true;
				}
			}
		} = false;
	}

	private sealed class Player(string name, string id)
	{
		public readonly string Name = name;
		public readonly string Id = id;
		public int? Shortcut;
		public Team? Team;

		public void Reset()
		{
			Shortcut = null;
			Team = null;
		}
	}

	private const int MatchDurationSec = 300;
	private readonly State State = state;
	private Match? _match;
	private Player? _player;
	private MessageSpeed _speed = new();
	private readonly Lock _gate = new();
	private bool _inReplay = false;

	internal void HandleEvent(Event apiEvent)
	{
		lock (_gate)
		{
			if (apiEvent.Type == EventType.ReplayPlaybackStart)
			{
				_inReplay = true;
			}
			if (apiEvent.Type == EventType.ReplayPlaybackEnd)
			{
				_inReplay = false;
			}
			else if (apiEvent.Type == EventType.MatchInitialized)
			{
				if (_match?.Guid != ((PayloadMatchInitialized)apiEvent.Payload).MatchGuid)
				{
					StopCurrentMatch();
					_match = new(((PayloadMatchInitialized)apiEvent.Payload).MatchGuid);
				}
				_match?.HasSTarted = true;
			}
			else if (apiEvent.Type == EventType.UpdateState)
			{
				UpdateState((PayloadUpdateState)apiEvent.Payload);
			}
			else if (apiEvent.Type == EventType.GoalScored && _inReplay == false)
			{
				GoalScored((PayloadGoalScored)apiEvent.Payload);
			}
			else if (apiEvent.Type == EventType.MatchEnded)
			{
				Log.Print("Match ended");
				_match?.WinnerSoFar = (Team?)((PayloadMatchEnded)apiEvent.Payload).WinnerTeamNum;
				StopCurrentMatch();
			}
			else if (apiEvent.Type == EventType.MatchDestroyed)
			{
				Log.Print("Match destroyed");
				StopCurrentMatch();
			}
			PrintSpeed();
		}
	}

	private static void GoalScored(PayloadGoalScored payload)
	{
		double startSpeed = Math.Round(payload.BallLastTouch.Speed);
		double goalSpeed = Math.Round(payload.GoalSpeed);

		Log.Print($"Goal scored: {startSpeed} km/h -> {goalSpeed} km/h");
	}

	private void UpdateState(PayloadUpdateState payload)
	{
		UpdateMatch(payload);
		UpdateMode(payload);
		UpdatePlayer(payload);
		UpdateScore(payload);
	}

	private void UpdateMatch(PayloadUpdateState payload)
	{
		if (_match?.Guid != payload.MatchGuid)
		{
			StopCurrentMatch();
			_match = new(payload.MatchGuid);
		}
		if (_match.HasSTarted == false && payload.Game.TimeSeconds < MatchDurationSec)
		{
			_match.HasSTarted = true;
		}
		if (payload.Game.BHasWinner == true)
		{
			_match.WinnerSoFar = GetWinnerTeam(payload);
		}
	}

	private void UpdateMode(PayloadUpdateState payload)
	{
		if (_match == null || payload.Players.Count == 1)
		{
			return;
		}
		int teamSize = (payload.Players.Count + 1) / 2;
		if (teamSize > (int)_match.Mode && teamSize < (int)GameMode.Count)
		{
			_match?.Mode = (GameMode)teamSize;
			Log.Print($"Mode selected:     {Log.Yellow}{_match?.Mode}");
		}
	}

	private void UpdatePlayer(PayloadUpdateState payload)
	{
		if (payload.Game.BReplay == false && payload.Game.Target != null
			&& _player?.Shortcut != payload.Game.Target.Shortcut)
		{
			ApiPlayer? apiPlayer = GetApiPlayer(payload);
			if (apiPlayer == null)
			{
				Log.PrintRed("Player not found");
				return;
			}

			if (_player == null || _player.Id != apiPlayer.PrimaryId)
			{
				_player = new(apiPlayer.Name, apiPlayer.PrimaryId);
				Log.Print($"Player selected: {Log.Yellow}{_player.Name}");
			}
			_player.Shortcut = apiPlayer.Shortcut;
			_player.Team = (Team)apiPlayer.TeamNum;
			Log.Print($"Player updated:  {Log.Yellow}Team {_player.Team} ({_player.Shortcut})");
		}
	}

	private void UpdateScore(PayloadUpdateState payload)
	{
		int blueScore = payload.Game.Teams[(int)Team.Blue].Score;
		int orangeScore = payload.Game.Teams[(int)Team.Orange].Score;

		if (blueScore > orangeScore)
		{
			_match?.WinnerSoFar = Team.Blue;
		}
		else if (orangeScore > blueScore)
		{
			_match?.WinnerSoFar = Team.Orange;
		}
		else
		{
			_match?.WinnerSoFar = Team.None;
		}
	}

	private static ApiPlayer? GetApiPlayer(PayloadUpdateState payload)
	{
		foreach (ApiPlayer apiPlayer in payload.Players)
		{
			if (apiPlayer.Shortcut == payload.Game.Target?.Shortcut)
				return apiPlayer;
		}
		return null;
	}

	private static Team? GetWinnerTeam(PayloadUpdateState payload)
	{
		foreach (ApiTeam team in payload.Game.Teams)
		{
			if (team.Name == payload.Game.Winner)
				return (Team)team.TeamNum;
		}
		return null;
	}

	private void StopCurrentMatch()
	{
		if (_match != null && _match.HasSTarted == true && _match.WinnerSoFar != null && _player != null && _player.Team != null)
		{
			string Mode = "???";
			if (_match.Mode == GameMode.OneVersusOne)
			{
				Mode = "1v1";
			}
			else if (_match.Mode == GameMode.TwoVersusTwo)
			{
				Mode = "2v2";
			}
			else if (_match.Mode == GameMode.ThreeVersusThree)
			{
				Mode = "3v3";
			}
			if (_player.Team == _match.WinnerSoFar)
			{
				State.PlusWin(_match.Mode);
				Log.PrintGreen($"=> [{Mode}] WIN!");
			}
			else
			{
				State.PlusLoss(_match.Mode);
				Log.PrintRed($"=> [{Mode}] LOSS");
			}
			Log.PrintBlue("-------------");
			Log.PrintBlue($" {State.CurrentTracker.Win} | {State.CurrentTracker.Loss} | {State.CurrentTracker.Streak}");
			Log.PrintBlue("-------------");
		}
		_player?.Reset();
		_inReplay = false;
	}

	private void PrintSpeed()
	{
		_speed.MessageCount++;
		_speed.TimeStartSec ??= DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		_speed.TimeLastSpeedPrint ??= DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		long _timeCurrSec = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		
		if (_timeCurrSec - _speed.TimeLastSpeedPrint >= MessageSpeed.SpeedPrintDelaySec)
		{
			double messagePerSec = _speed.MessageCount / (double)(_timeCurrSec - _speed.TimeStartSec);
			Log.PrintBlue($"[{messagePerSec}/sec]");
			_speed.TimeLastSpeedPrint = _timeCurrSec;
		}
	}
}
