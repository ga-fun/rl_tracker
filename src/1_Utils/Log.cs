using System.Runtime.CompilerServices;
using System.Text.Json;

namespace GuillaumeAst.Utils;

public static class Log
{	
	public enum Level
	{
		Debug,
		Info,
		Warning,
		Error
	}

	private const string FileExtension = ".log";
	private const uint MaxFileCount = 10;
	private const uint MaxFileSizeInMo = 100;
	private const uint MaxFileAgeInDays = 7;
	private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
	private static readonly Lock FileGate = new();
	private static readonly string Dir = Path.Combine(App.Directory, "logs");
	private static readonly string FileName = $"{DateTime.Now:dd-MM-yy-HH:mm:ss}{FileExtension}";
	public static Level LevelMin { get; set; } = Level.Info;
	private static bool Enabled { get; set; } = false;
	public static readonly string LogFile = Path.Combine(Dir, FileName);

	public static void Init(Level levelMin = Level.Info)
	{
		LevelMin = levelMin;
		try
		{
			Directory.CreateDirectory(Dir);
			Cleanup();
			Enabled = true;
		}
		catch (Exception exception) when (exception
			is IOException
			or UnauthorizedAccessException
			or PathTooLongException
  			or DirectoryNotFoundException
			or NotSupportedException)
		{
			Enabled = false;
		}
	}

	public static void Dump<T>(
		T value,
		Level level,
		string? message = null,
		[CallerMemberName] string caller = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		string dump;
	
		if (Enabled == false)
		{
			return;
		}
		try
		{
			dump = JsonSerializer.Serialize(value, JsonOptions);
		}
		catch (NotSupportedException exception)
		{
			dump = $"Exception while serializing: {exception.Message}";
		}
		dump = message ?? "Dump:" + "\n" + dump;
		WriteInternal(level, dump, caller, file, line);
	}

	public static void Write(
		Level level,
		string message,
		[CallerMemberName] string caller = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		if (Enabled == false)
		{
			return;
		}
		WriteInternal(level, message, caller, file, line);
	}

	private static void WriteInternal(
		Level level,
		string message,
		string caller,
		string file,
		int line)
	{
		if (Enabled == false)
		{
			return;
		}

		string fileName = Path.GetFileName(file);
		string time = $"{DateTime.Now:HH:mm:ss.fff}";
		// TODO (START): only for console debugging
		string color = Reset;
		if (level == Level.Warning)
		{
			color = Yellow;
		}
		else if (level == Level.Error)
		{
			color = Red;
		}
		// TODO (STOP): only for console debugging
		string fmessage = $"[{color}{level}{Reset} | {time} {fileName}:{line}:{caller}] {color}{message}{Reset}";
		string log = RemoveColors(fmessage);	// TODO: only for console debugging
		try
		{
			lock (FileGate)
			{
				// TODO (START): only for console debugging
				if (level != Level.Debug)
				{
					PrintInternal(fmessage + Reset, caller, file, line);
				}
				// TODO (STOP): only for console debugging
				File.AppendAllText(LogFile, log + Environment.NewLine);
			}
		}
		catch (Exception exception) when (exception
			is PathTooLongException
  			or DirectoryNotFoundException
			or IOException
			or UnauthorizedAccessException
			or NotSupportedException
			or System.Security.SecurityException)
		{
			Enabled = false;
		}
	}

	private static void Cleanup()
	{
		try
		{
			List<FileInfo> files = [.. Directory
				.EnumerateFiles(Dir, $"*{FileExtension}")
				.Select(path => new FileInfo(path))
				.OrderByDescending(file => file.LastWriteTimeUtc)];
			
			while (files.Count > MaxFileCount
				|| files.Sum(file => file.Length) > MaxFileSizeInMo
				|| files[^1].LastWriteTimeUtc < DateTime.UtcNow.AddDays(-(double)MaxFileAgeInDays))
			{
				files[^1].Delete();
				files.RemoveAt(files.Count - 1);
			}
		}
		catch (Exception exception) when (exception
  			is DirectoryNotFoundException
			or IOException
			or PathTooLongException
			or System.Security.SecurityException
			or UnauthorizedAccessException
			or OverflowException)
		{
			// Best effort
		}
	}

	// TODO (START): only for console debugging
	public const string Green = "\u001b[32m";
	public const string Blue = "\u001b[34m";
	public const string Red = "\u001b[31m";
	public const string Yellow = "\u001b[33m";
	public const string Reset = "\u001b[0m";

	private static void PrintInternal(string message, string caller, string file, int line)
	{
		string fileName = Path.GetFileName(file);
		string time = DateTime.Now.ToString("HH:mm:ss.fff");
		Console.WriteLine($"[{time} {fileName}:{line}:{caller}] {message}{Reset}");
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
	// TODO (END): only for console debugging
}
