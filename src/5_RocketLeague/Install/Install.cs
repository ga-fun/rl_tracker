using System.Text.Json.Serialization;
using GuillaumeAst.Utils;

namespace GuillaumeAst.RocketLeague;

public abstract class Install : Notifier
{
	protected static readonly string ProgramFilesDir = GetSpecialFolderOrEmpty(
		Environment.SpecialFolder.ProgramFiles);
	protected static readonly string ProgramFilesX86Dir = GetSpecialFolderOrEmpty(
		Environment.SpecialFolder.ProgramFilesX86);
	protected const string GamesDirName = "Games";

	private static string GetSpecialFolderOrEmpty(Environment.SpecialFolder folder)
	{
		try
		{
			return Environment.GetFolderPath(folder);
		}
		catch (PlatformNotSupportedException)
		{
			return "";
		}
	}

	public string? InstallDir
	{
		get;
		set
		{
			if (field == value)
			{
				return;
			}
			if (value == null || !InstallDirIsValid(value))
			{
				AutoDetectInstallDir();
			}
			else
			{
				field = value;
			}
			IsValid = InstallDirIsValid(InstallDir);
			NotifyChange();
		}
	}

	[JsonIgnore]
	public bool IsValid
	{
		get;
		protected set
		{
			if (field != value)
			{
				field = value;
				NotifyChange();
			}
		}
	}

	public abstract void AutoDetectInstallDir();

	public static bool InstallDirIsValid(string? installDir)
	{
		return !string.IsNullOrWhiteSpace(installDir)
			&& Directory.Exists(installDir)
			&& File.Exists(Path.Combine(
				installDir,
				StatsApi.Config.ConfigFileRelativePath
			));
	}

	protected static DriveInfo[] GetDrivesOrEmpty()
	{
		try
		{
			return DriveInfo.GetDrives();
		}
		catch (IOException)
		{
			return [];
		}
		catch (UnauthorizedAccessException)
		{
			return [];
		}
	}
}
