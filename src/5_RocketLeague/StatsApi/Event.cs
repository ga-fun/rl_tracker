using System.Text.Json;

namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class Event
{
	private static readonly Dictionary<EventType, Func<JsonDocument, IPayload>>
		PayloadParsers = new()
		{
			{ EventType.UpdateState, ParsePayload<PayloadUpdateState> },
			{ EventType.BallHit, ParsePayload<PayloadBallHit> },
			{ EventType.ClockUpdatedSeconds, ParsePayload<PayloadClockUpdatedSeconds> },
			{ EventType.CountdownBegin, ParsePayload<PayloadCountdownBegin> },
			{ EventType.CrossbarHit, ParsePayload<PayloadCrossbarHit> },
			{ EventType.ReplayPlaybackEnd, ParsePayload<PayloadReplayPlaybackEnd> },
			{ EventType.ReplayPlaybackStart, ParsePayload<PayloadReplayPlaybackStart> },
			{ EventType.ReplayWillEnd, ParsePayload<PayloadReplayWillEnd> },
			{ EventType.GoalScored, ParsePayload<PayloadGoalScored> },
			{ EventType.MatchCreated, ParsePayload<PayloadMatchCreated> },
			{ EventType.MatchInitialized, ParsePayload<PayloadMatchInitialized> },
			{ EventType.MatchDestroyed, ParsePayload<PayloadMatchDestroyed> },
			{ EventType.MatchEnded, ParsePayload<PayloadMatchEnded> },
			{ EventType.MatchPaused, ParsePayload<PayloadMatchPaused> },
			{ EventType.MatchUnpaused, ParsePayload<PayloadMatchUnpaused> },
			{ EventType.PodiumStart, ParsePayload<PayloadPodiumStart> },
			{ EventType.ReplayCreated, ParsePayload<PayloadReplayCreated> },
			{ EventType.RoundStarted, ParsePayload<PayloadRoundStarted> },
			{ EventType.StatfeedEvent, ParsePayload<PayloadStatfeedEvent> }
		};

	public EventType Type { get; }
	public IPayload Payload { get; }

	public Event(string rawMessage)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(rawMessage);
		JsonDocument document;
		try
		{
			document = JsonDocument.Parse(rawMessage);
		}
		catch (JsonException exception)
		{
			throw new FormatException("Invalid message: must be JSON encoded.", exception);
		}
		using (document)
		{
			if (document.RootElement.ValueKind != JsonValueKind.Object)
			{
				throw new FormatException("Invalid message: Root JSON value must be an object");
			}
			Type = ParseType(document);
			if (!PayloadParsers.TryGetValue(Type, out Func<JsonDocument, IPayload>? parser))
			{
				throw new NotSupportedException($"Unsupported event: \"{Type}\"");
			}
			Payload = parser(document);
		}
	}

	private static EventType ParseType(JsonDocument document)
	{
		const string field = "Event";
		string value;

		if (!document.RootElement.TryGetProperty(field, out JsonElement property))
		{
			throw new FormatException($"Missing field: \"{field}\"");
		}
		if (property.ValueKind != JsonValueKind.String)
		{
			throw new FormatException($"Invalid field: \"{field}\" is not a string");
		}
		value = property.GetString()
			?? throw new FormatException($"Invalid field: \"{field}\"");
		if (!Enum.TryParse(value, out EventType type))
		{
			throw new FormatException($"Invalid \"{field}\": \"{value}\"");
		}
		if (Enum.GetName(type) != value)
		{
			throw new FormatException($"Invalid \"{field}\": \"{value}\"");
		}
		return type;
	}

	private static IPayload ParsePayload<T>(JsonDocument document)
		where T : IPayload
	{
		const string field = "Data";
		string json;
		T payload;

		if (!document.RootElement.TryGetProperty(field, out JsonElement property))
		{
			throw new FormatException($"Missing field: \"{field}\"");
		}
		if (property.ValueKind != JsonValueKind.String)
		{
			throw new FormatException($"Invalid field: \"{field}\" is not a string");
		}
		json = property.GetString()
			?? throw new FormatException($"Invalid field: \"{field}\"");
		try
		{
			payload = JsonSerializer.Deserialize<T>(json)
				?? throw new FormatException($"Invalid payload: {typeof(T).Name}");
		}
		catch (JsonException exception)
		{
			throw new FormatException($"Invalid payload: {typeof(T).Name}.", exception);
		}
		return payload;
	}
}
