using System.Text.Json.Serialization;
using System.Text.Json;
using GuillaumeAst.Utils;

namespace GuillaumeAst.RocketLeague;

public sealed class InstallEpic : Install
{
	private const string EpicGamesDirName = "Epic Games";
	private const string EpicRlDirName = "rocketleague";
	private const string EpicManifestPattern = "*.item";
	private const string EpicInstallLocationProperty = "InstallLocation";
	private static readonly string? ManifestsDir = GetManifestsDir();

	private static string? GetManifestsDir()
	{
		string commonAppData;

		try
		{
			commonAppData = Environment.GetFolderPath(
				Environment.SpecialFolder.CommonApplicationData);
		}
		catch (PlatformNotSupportedException)
		{
			return null;
		}

		if (string.IsNullOrWhiteSpace(commonAppData))
		{
			return null;
		}
		return Path.Combine(
			commonAppData,
			"Epic",
			"EpicGamesLauncher",
			"Data",
			"Manifests");
	}

	[JsonConstructor]
	private InstallEpic(string? installDir)
	{
		InstallDir = installDir;
		IsValid = InstallDirIsValid(installDir);
	}

	public InstallEpic()
	{
		InstallDir = null;
		IsValid = false;
	}

	public override void AutoDetectInstallDir()
	{
		if (TryFindFromManifests() || TryFindFromClassicPaths())
		{
			Log.Write(Log.Level.Info, $"RL Epic dir detected:\"{InstallDir}\"");
		}
		else
		{
			Log.Write(Log.Level.Warning, "RL Epic dir not detected");
		}
	}

	private bool TryFindFromManifests()
	{
		if (string.IsNullOrWhiteSpace(ManifestsDir))
		{
			return false;
		}
		try
		{
			foreach (string manifestFile in Directory.EnumerateFiles(ManifestsDir, EpicManifestPattern))
			{
				if (TryReadInstallDirInManifest(manifestFile, out string installDir)
					&& InstallDirIsValid(installDir))
				{
					InstallDir = installDir;
					return true;
				}
			}
		}
		catch (Exception exception) when (exception
			is IOException
			or UnauthorizedAccessException
			or System.Security.SecurityException)
		{
			return false;
		}
		return false;
	}

	private static bool TryReadInstallDirInManifest(string manifestFile, out string installDir)
	{
		installDir = "";
		try
		{
			string json = File.ReadAllText(manifestFile);
			using JsonDocument document = JsonDocument.Parse(json);

			if (document.RootElement.ValueKind != JsonValueKind.Object)
			{
				return false;
			}
			if (!document.RootElement.TryGetProperty(EpicInstallLocationProperty, out JsonElement property))
			{
				return false;
			}
			if (property.ValueKind != JsonValueKind.String)
			{
				return false;
			}
			installDir = (property.GetString() ?? "").Trim();
			return installDir.Length > 0;
		}
		catch (Exception exception) when (exception
			is IOException
			or UnauthorizedAccessException
			or NotSupportedException
			or System.Security.SecurityException
			or JsonException)
		{
			return false;
		}
	}

	private bool TryFindFromClassicPaths()
	{
		foreach (string epicRoot in GetRootCandidates())
		{
			string installDir = Path.Combine(epicRoot, EpicRlDirName);

			if (InstallDirIsValid(installDir))
			{
				InstallDir = installDir;
				return true;
			}
		}
		return false;
	}

	private static IEnumerable<string> GetRootCandidates()
	{
		if (!string.IsNullOrWhiteSpace(ProgramFilesDir))
		{
			yield return Path.Combine(ProgramFilesDir, EpicGamesDirName);
		}
		if (!string.IsNullOrWhiteSpace(ProgramFilesX86Dir))
		{
			yield return Path.Combine(ProgramFilesX86Dir, EpicGamesDirName);
		}
		foreach (DriveInfo drive in GetDrivesOrEmpty())
		{
			if (!drive.IsReady)
			{
				continue;
			}
			string driveRootDir;
			try
			{
				driveRootDir = drive.RootDirectory.FullName;
			}
			catch (PathTooLongException)
			{
				continue;
			}
			catch (System.Security.SecurityException)
			{
				continue;
			}
			yield return Path.Combine(driveRootDir, EpicGamesDirName);
			yield return Path.Combine(driveRootDir, GamesDirName);
			yield return Path.Combine(driveRootDir, GamesDirName, EpicGamesDirName);
		}
	}
}
