using System.Text.Json;

namespace RlStatsApi;

internal sealed class Event
{
	public Type Type { get; }
	public Payload Payload { get; }

	private static readonly Dictionary<Type, Func<JsonDocument, Payload>>
		PayloadParsers = new()
		{
			{ Type.UpdateState, ParsePayload<PayloadUpdateState> },
			{ Type.BallHit, ParsePayload<PayloadBallHit> },
			{ Type.ClockUpdatedSeconds, ParsePayload<PayloadClockUpdatedSeconds> },
			{ Type.CountdownBegin, ParsePayload<PayloadCountdownBegin> },
			{ Type.CrossbarHit, ParsePayload<PayloadCrossbarHit> },
			{ Type.GoalReplayEnd, ParsePayload<PayloadGoalReplayEnd> },
			{ Type.GoalReplayStart, ParsePayload<PayloadGoalReplayStart> },
			{ Type.GoalReplayWillEnd, ParsePayload<PayloadGoalReplayWillEnd> },
			{ Type.GoalScored, ParsePayload<PayloadGoalScored> },
			{ Type.MatchCreated, ParsePayload<PayloadMatchCreated> },
			{ Type.MatchInitialized, ParsePayload<PayloadMatchInitialized> },
			{ Type.MatchDestroyed, ParsePayload<PayloadMatchDestroyed> },
			{ Type.MatchEnded, ParsePayload<PayloadMatchEnded> },
			{ Type.MatchPaused, ParsePayload<PayloadMatchPaused> },
			{ Type.MatchUnpaused, ParsePayload<PayloadMatchUnpaused> },
			{ Type.PodiumStart, ParsePayload<PayloadPodiumStart> },
			{ Type.ReplayCreated, ParsePayload<PayloadReplayCreated> },
			{ Type.RoundStarted, ParsePayload<PayloadRoundStarted> },
			{ Type.StatfeedEvent, ParsePayload<PayloadStatfeedEvent> }
		};

	public Event(string rawMessage)
	{
		ArgumentNullException.ThrowIfNull(rawMessage);

		using JsonDocument document = JsonDocument.Parse(rawMessage);

		Type = ParseType(document);
		if (!PayloadParsers.TryGetValue(Type, out Func<JsonDocument, Payload>? parser))
			throw new InvalidOperationException($"Unsupported event: {Type}.");
		Payload = parser(document);
	}

	private static Type ParseType(JsonDocument document)
	{
		const string field = "Event";
		string value;

		if (!document.RootElement.TryGetProperty(field, out JsonElement property))
			throw new FormatException($"Missing field: \"{field}\".");
		value = property.GetString()
			?? throw new FormatException($"Invalid field: \"{field}\".");
		if (!Enum.TryParse(value, out Type type))
			throw new FormatException($"Invalid \"{field}\": {value}.");
		if (Enum.GetName(type) != value)
			throw new FormatException($"Invalid \"{field}\": {value}.");
		return type;
	}

	private static Payload ParsePayload<T>(JsonDocument document)
		where T : Payload
	{
		const string field = "Data";

		if (!document.RootElement.TryGetProperty(field, out JsonElement property))
			throw new FormatException($"Missing field: \"{field}\".");
		if (property.ValueKind != JsonValueKind.Object)
			throw new FormatException($"Invalid field: \"{field}\" is not an object.");
		T payload = JsonSerializer.Deserialize<T>(property)
			?? throw new FormatException($"Failed to parse payload: {typeof(T).Name}.");
		return payload;
	}
}
