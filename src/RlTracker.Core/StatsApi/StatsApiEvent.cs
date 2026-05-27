using System.Text.Json;

namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiEvent
{
	public StatsApiEventType Type { get; }
	public StatsApiPayload Payload { get; }

	private static readonly Dictionary<StatsApiEventType, Func<JsonDocument, StatsApiPayload>>
		PayloadParsers = new()
		{
			{ StatsApiEventType.UpdateState, ParsePayload<StatsApiPayloadUpdateState> },
			{ StatsApiEventType.BallHit, ParsePayload<StatsApiPayloadBallHit> },
			{ StatsApiEventType.ClockUpdatedSeconds, ParsePayload<StatsApiPayloadClockUpdatedSeconds> },
			{ StatsApiEventType.CountdownBegin, ParsePayload<StatsApiPayloadCountdownBegin> },
			{ StatsApiEventType.CrossbarHit, ParsePayload<StatsApiPayloadCrossbarHit> },
			{ StatsApiEventType.GoalReplayEnd, ParsePayload<StatsApiPayloadGoalReplayEnd> },
			{ StatsApiEventType.GoalReplayStart, ParsePayload<StatsApiPayloadGoalReplayStart> },
			{ StatsApiEventType.GoalReplayWillEnd, ParsePayload<StatsApiPayloadGoalReplayWillEnd> },
			{ StatsApiEventType.GoalScored, ParsePayload<StatsApiPayloadGoalScored> },
			{ StatsApiEventType.MatchCreated, ParsePayload<StatsApiPayloadMatchCreated> },
			{ StatsApiEventType.MatchInitialized, ParsePayload<StatsApiPayloadMatchInitialized> },
			{ StatsApiEventType.MatchDestroyed, ParsePayload<StatsApiPayloadMatchDestroyed> },
			{ StatsApiEventType.MatchEnded, ParsePayload<StatsApiPayloadMatchEnded> },
			{ StatsApiEventType.MatchPaused, ParsePayload<StatsApiPayloadMatchPaused> },
			{ StatsApiEventType.MatchUnpaused, ParsePayload<StatsApiPayloadMatchUnpaused> },
			{ StatsApiEventType.PodiumStart, ParsePayload<StatsApiPayloadPodiumStart> },
			{ StatsApiEventType.ReplayCreated, ParsePayload<StatsApiPayloadReplayCreated> },
			{ StatsApiEventType.RoundStarted, ParsePayload<StatsApiPayloadRoundStarted> },
			{ StatsApiEventType.StatfeedEvent, ParsePayload<StatsApiPayloadStatfeedEvent> }
		};

	public StatsApiEvent(string rawMessage)
	{
		ArgumentNullException.ThrowIfNull(rawMessage);

		using JsonDocument document = JsonDocument.Parse(rawMessage);

		Type = ParseEventType(document);
		if (!PayloadParsers.TryGetValue(Type, out Func<JsonDocument, StatsApiPayload>? parser))
			throw new InvalidOperationException($"Unsupported event: {Type}.");
		Payload = parser(document);
	}

	private static StatsApiEventType ParseEventType(JsonDocument document)
	{
		const string field = "Event";
		string value;

		if (!document.RootElement.TryGetProperty(field, out JsonElement property))
			throw new FormatException($"Missing field: \"{field}\".");
		value = property.GetString()
			?? throw new FormatException($"Invalid field: \"{field}\".");
		if (!Enum.TryParse(value, out StatsApiEventType type))
			throw new FormatException($"Invalid \"{field}\": {value}.");
		if (Enum.GetName(type) != value)
			throw new FormatException($"Invalid \"{field}\": {value}.");
		return type;
	}

	private static StatsApiPayload ParsePayload<T>(JsonDocument document)
		where T : StatsApiPayload
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
