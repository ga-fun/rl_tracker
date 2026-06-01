using GuillaumeAst.Utils;
using GuillaumeAst.RocketLeague.StatsApi;
using ApiPlayer = GuillaumeAst.RocketLeague.StatsApi.Player;

namespace GuillaumeAst.RlTracker.Core;

internal sealed class MessageHandler(State state)
{
	private sealed class MessageSpeed
	{
		public const long SpeedPrintDelaySec = 300;
		public long? TimeStartSec = null;
		public long MessageCount = 0;
		public long TimeLastSpeedPrint = 0;
	}

	public sealed class Match(string guid)
	{
		public string Guid = string.IsNullOrWhiteSpace(guid)
			? throw new ArgumentException("Match guid must not be null or white-space.", nameof(guid))
			: guid;
		public GameMode mode = GameMode.Other;
		public Team? winnerSoFar;
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

	private readonly State State = state;
	private Match? _match;
	private Player? _player;
	private MessageSpeed _speed = new();
	private readonly Lock _gate = new();

	internal void HandleEvent(Event apiEvent)
	{
		lock (_gate)
		{
			if (apiEvent.Type == EventType.MatchInitialized)
			{
				StopCurrentMatch();
				_match = new(((PayloadMatchInitialized)apiEvent.Payload).MatchGuid);
				Log.Print("----------------------------");
				Log.Print("Match initialized:");
			}
			else if (_match == null)
			{
				return;
			}
			else if (apiEvent.Type == EventType.MatchEnded)
			{
				Log.Print("Match ended.");
				_match?.winnerSoFar = (Team?)((PayloadMatchEnded)apiEvent.Payload).WinnerTeamNum;
				StopCurrentMatch();
			}
			else if (apiEvent.Type == EventType.MatchDestroyed)
			{
				Log.Print("Match destroyed.");
				StopCurrentMatch();
			}
			else if (apiEvent.Type == EventType.UpdateState)
			{
				UpdateState((PayloadUpdateState)apiEvent.Payload);
			}
			else if (apiEvent.Type == EventType.GoalScored)
			{
				GoalScored((PayloadGoalScored)apiEvent.Payload);
			}
			PrintSpeed();
		}
	}

	// TODO: tmp
	private static void GoalScored(PayloadGoalScored payload)
	{
		double startSpeed = payload.BallLastTouch.Speed * 0.036;
		double goalSpeed = payload.GoalSpeed * 0.036;

		Log.Print($"Goal scored: {startSpeed} km/h -> {goalSpeed} km/h.");
	}

	private void UpdateState(PayloadUpdateState payload)
	{
		if (_match == null)
		{
			_match = new(payload.MatchGuid);
		}
		else if (_match.Guid != payload.MatchGuid)
		{
			StopCurrentMatch();
			_match = new(payload.MatchGuid);
		}
		SetGameMode(payload);
		UpdatePlayer(payload);
		UpdateScore(payload);
	}

	private void SetGameMode(PayloadUpdateState payload)
	{
		if (_match?.mode != GameMode.Other)
		{
			return;
		}

		int teamSize = payload.Players.Count / 2;
		if (teamSize >= (int)GameMode.OneVersusOne && teamSize <= (int)GameMode.ThreeVersusThree)
		{
			_match?.mode = (GameMode)teamSize;
		}
		else
		{
			_match?.mode = GameMode.Other;
		}
		Log.Print($"Mode selected: {_match?.mode}");
	}

	private void UpdatePlayer(PayloadUpdateState payload)
	{
		if (payload.Game.BReplay == false && payload.Game.Target != null
			&& _player?.Shortcut != payload.Game.Target.Shortcut)
		{
			ApiPlayer? apiPlayer = GetApiPlayer(payload);
			if (apiPlayer == null)
			{
				Log.PrintRed("Player not found.");
				return;
			}

			if (_player == null || _player.Id != apiPlayer.PrimaryId)
			{
				_player = new(apiPlayer.Name, apiPlayer.PrimaryId);
				Log.PrintGreen($"Player selected: {_player.Name}.");
			}
			_player.Shortcut = apiPlayer.Shortcut;
			_player.Team = (Team)apiPlayer.TeamNum;
			Log.PrintYellow($"Player updated: Team {_player.Team} ({_player.Shortcut}).");
		}
	}

	private void UpdateScore(PayloadUpdateState payload)
	{
		Team previousWinner = _match?.winnerSoFar ?? Team.None;
		int blueScore = payload.Game.Teams[(int)Team.Blue].Score;
		int orangeScore = payload.Game.Teams[(int)Team.Orange].Score;

		if (blueScore > orangeScore)
		{
			_match?.winnerSoFar = Team.Blue;
		}
		else if (orangeScore > blueScore)
		{
			_match?.winnerSoFar = Team.Orange;
		}
		else
		{
			_match?.winnerSoFar = Team.None;
		}
		if (_match?.winnerSoFar != previousWinner)
		{
			Log.Print($"Winner so far = {_match?.winnerSoFar} [{Log.Blue}{blueScore}{Log.Reset} - {Log.Yellow}{orangeScore}{Log.Reset}].");
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

	private void StopCurrentMatch()
	{
		if (_match != null && _match.winnerSoFar != null && _player != null && _player.Team != null)
		{
			if (_player.Team == _match.winnerSoFar)
			{
				State.PlusWin(_match.mode);
				Log.PrintGreen($"===> WIN! <===");
			}
			else
			{
				State.PlusLoss(_match.mode);
				Log.PrintRed($"===> LOSS <===");
			}
			Log.PrintBlue("----------------------------");
			Log.PrintBlue($" {State.CurrentTracker.Win} - {State.CurrentTracker.Loss} - {State.CurrentTracker.Streak}");
			Log.PrintBlue("----------------------------");
		}
		_match = null;
		_player?.Reset();
	}

	private void PrintSpeed()
	{
		_speed.TimeStartSec ??= DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		long _timeCurrSec = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		_speed.MessageCount++;
		double messagePerSec = _speed.MessageCount / (double)(_timeCurrSec - _speed.TimeStartSec);
		if (_timeCurrSec - _speed.TimeLastSpeedPrint >= MessageSpeed.SpeedPrintDelaySec)
		{
			Log.PrintBlue($"[{messagePerSec}/sec]");
			_speed.TimeLastSpeedPrint = _timeCurrSec;
		}
	}
}
