using System.Runtime.CompilerServices;
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

	public static void Dump<T>(
		T value,
		string? message = null,
		[CallerMemberName] string caller = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		string label = message ?? "Dump:";
		string dump;
	
		PrintPriv(caller, file, line, label);
		try
		{
			dump = JsonSerializer.Serialize(value, JsonOptions);
		}
		catch (Exception exception)
		{
			dump = $"Exception while serializing: {exception.Message}";
		}
		Console.WriteLine(dump);
	}

	public static void Print(
		string message,
		[CallerMemberName] string caller = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		PrintPriv(caller, file, line, message);
	}

	public static void PrintGreen(
		string message,
		[CallerMemberName] string caller = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		PrintPriv(caller, file, line, Green + message);
	}

	public static void PrintBlue(
		string message,
		[CallerMemberName] string caller = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		PrintPriv(caller, file, line, Blue + message);
	}

	public static void PrintRed(
		string message,
		[CallerMemberName] string caller = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		PrintPriv(caller, file, line, Red + message);
	}

	public static void PrintYellow(
		string message,
		[CallerMemberName] string caller = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		PrintPriv(caller, file, line, Yellow + message);
	}

	private static void PrintPriv(string caller, string file, int line, string message)
	{
		string filename = Path.GetFileName(file);
		Console.WriteLine($"[{filename}:{line}:{caller}] {message}{Reset}");
	}
}
