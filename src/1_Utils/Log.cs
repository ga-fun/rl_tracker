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
	private static readonly string FileName = $"{DateTime.Now:dd-MM-yy-HH-mm-ss}{FileExtension}";
	public static Level LevelMin { get; set; } = Level.Info;
	private static bool Enabled { get; set; } = false;
	public static readonly string LogFile = Path.Combine(Dir, FileName);

	public static void Init()
	{
		Enabled = true;
		try
		{
			Directory.CreateDirectory(Dir);
			Write(Level.Info, $"Logs will be stored in: \"{LogFile}\"");
			Cleanup();
			Write(Level.Debug, $"Log init succeed (Enabled = {Enabled} | LevelMin = {LevelMin})");
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
	
		if (ShouldWrite(level) == false)
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
		if (ShouldWrite(level))
		{
			WriteInternal(level, message, caller, file, line);
		}
	}

	private static bool ShouldWrite(Level level)
	{
		return Enabled == true && level >= LevelMin;
	}

	private static void WriteInternal(
		Level level,
		string message,
		string caller,
		string file,
		int line)
	{
		string fileName = Path.GetFileName(file);
		string time = $"{DateTime.Now:HH:mm:ss.fff}";
		string fmessage = $"[{level} | {time} {fileName}:{line}:{caller}] {message}";
		try
		{
			lock (FileGate)
			{
				File.AppendAllText(LogFile, fmessage + Environment.NewLine);
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
			
			while (ShouldDeleteOldestFile(files))
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

	private static bool ShouldDeleteOldestFile(List<FileInfo> files)
	{
		if (files.Count == 0)
		{
			return false;
		}
		return files.Count > MaxFileCount
			|| files.Sum(file => file.Length) > MaxFileSizeInMo * 1024 * 1024
			|| files[^1].LastWriteTimeUtc < DateTime.UtcNow.AddDays(-(double)MaxFileAgeInDays);
	}
}
