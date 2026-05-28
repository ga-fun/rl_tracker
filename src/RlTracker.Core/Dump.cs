using System.Reflection.Metadata;
using System.Text.Json;

namespace RlTracker.Core;

public static class Log
{
	private static readonly JsonSerializerOptions JsonOptions = new(){ WriteIndented = true };
	public const string Green = "\u001b[32m";
	public const string Blue = "\u001b[34m";
	public const string Red = "\u001b[31m";
	public const string Yellow = "\u001b[33m";
	public const string Reset = "\u001b[0m";

	public static void Dump<T>(T value)
	{
		Console.WriteLine(JsonSerializer.Serialize(value, JsonOptions));
	}
}
