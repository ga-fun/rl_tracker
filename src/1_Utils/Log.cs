using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace GuillaumeAst.Utils;

public static class Log
{
	public static readonly string LogFile = GetLogFile();
	public const string Green = "\u001b[32m";
	public const string Blue = "\u001b[34m";
	public const string Red = "\u001b[31m";
	public const string Yellow = "\u001b[33m";
	public const string Reset = "\u001b[0m";

	private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
	private static readonly Lock FileGate = new();
	private const string LogDirName = "logs";
	private const string LogFileName = "rlTracker.log";

	private static string GetLogFile()
	{
		string? LogRootDir = null;
		try
		{
			LogRootDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		}
		catch (PlatformNotSupportedException)
		{
			// Fallback available
		}
		if (string.IsNullOrWhiteSpace(LogRootDir))
		{
			try
			{
				LogRootDir = Environment.GetEnvironmentVariable("HOME");
			}
			catch (System.Security.SecurityException)
			{
				// Fallback available
			}
		}
		if (string.IsNullOrWhiteSpace(LogRootDir))
		{
			try
			{
				LogRootDir = Environment.GetEnvironmentVariable("USERPROFILE");
			}
			catch (System.Security.SecurityException)
			{
				// Fallback available
			}
		}
		if (string.IsNullOrWhiteSpace(LogRootDir))
		{
			return Path.Combine(AppContext.BaseDirectory, LogFileName);
		}
		return Path.Combine(LogRootDir, LogDirName, LogFileName);
	}

	public static void Dump<T>(
		T value,
		string? message = null,
		[CallerMemberName] string caller = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		string label = message ?? "Dump:";
		string dump;
	
		PrintInternal(caller, file, line, label);
		try
		{
			dump = JsonSerializer.Serialize(value, JsonOptions);
		}
		catch (NotSupportedException exception)
		{
			dump = $"Exception while serializing: {exception.Message}";
		}
		Console.WriteLine(dump);
	}

	public static void Write(
		string message,
		[CallerMemberName] string caller = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		WriteInternal(caller, file, line, message);
	}

	public static void Print(
		string message,
		[CallerMemberName] string caller = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		PrintInternal(caller, file, line, message);
	}

	public static void PrintGreen(
		string message,
		[CallerMemberName] string caller = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		PrintInternal(caller, file, line, Green + message);
	}

	public static void PrintBlue(
		string message,
		[CallerMemberName] string caller = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		PrintInternal(caller, file, line, Blue + message);
	}

	public static void PrintRed(
		string message,
		[CallerMemberName] string caller = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		PrintInternal(caller, file, line, Red + message);
	}

	public static void PrintYellow(
		string message,
		[CallerMemberName] string caller = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		PrintInternal(caller, file, line, Yellow + message);
	}

	private static void PrintInternal(string caller, string file, int line, string message)
	{
		string fileName = Path.GetFileName(file);
		string time = DateTime.Now.ToString("HH:mm:ss.fff");
		Console.WriteLine($"[{time} {fileName}:{line}:{caller}] {message}{Reset}");
		WriteInternal(caller, file, line, message);
	}

	private static void WriteInternal(string caller, string file, int line, string message)
	{
		string fileName = Path.GetFileName(file);
		string time = DateTime.Now.ToString("HH:mm:ss.fff");
		string log = RemoveColors($"[{time} {fileName}:{line}:{caller}] {message}");

		try
		{
			lock (FileGate)
			{
				string? directory = Path.GetDirectoryName(LogFile);

				if (!string.IsNullOrWhiteSpace(directory))
				{
					Directory.CreateDirectory(directory);
				}
				File.AppendAllText(LogFile, log + Environment.NewLine);
			}
		}
		catch (Exception exception) when (exception
			is IOException
			or UnauthorizedAccessException
			or NotSupportedException
			or System.Security.SecurityException)
		{
			// Classic IO Exceptions: best effort
		}
	}

	private static string RemoveColors(string value)
	{
		return value
			.Replace(Green, "")
			.Replace(Blue, "")
			.Replace(Red, "")
			.Replace(Yellow, "")
			.Replace(Reset, "");
	}
}
