using System.Text.Json.Serialization;

namespace RlTracker.Core;

public abstract class RlInstall(string? installDir) : Notifier
{
	public static readonly string ProgramFilesDir = Environment.GetFolderPath(
		Environment.SpecialFolder.ProgramFiles);
	public static readonly string ProgramFilesX86Dir = Environment.GetFolderPath(
		Environment.SpecialFolder.ProgramFilesX86);
	public const string GamesDirName = "Games";
	public string? InstallDir
	{
		get;
		set
		{
			if (field == value)
				return;
			field = value;
			if (value == null)
				FindInstallDir();
			IsValid = InstallDirIsValid(value);
			NotifyChange();
		}
	} = installDir;

	[JsonIgnore]
	public bool IsValid
	{
		get;
		private set
		{
			if (field == value)
				return;
			field = value;
			NotifyChange();
		}
	}

	public abstract void FindInstallDir();

	public static bool InstallDirIsValid(string? installDir)
	{
		return !string.IsNullOrWhiteSpace(installDir)
			&& Directory.Exists(installDir)
			&& File.Exists(Path.Combine(
				installDir,
				RlStatsApi.Config.ConfigFileRelativePath
			));
	}
}
