using System.Text.Json.Serialization;
using GuillaumeAst.Utils;
using GuillaumeAst.FileVdf;

namespace GuillaumeAst.RocketLeague;

public sealed class InstallSteam : Install
{
	private const string SteamDirName = "Steam";
	private const string SteamLibraryDirName = "SteamLibrary";
	private const string SteamAppsDirName = "steamapps";
	private const string SteamCommonDirName = "common";
	private const string SteamRlDirName = "rocketleague";
	private const string SteamManifestFileName = "appmanifest_252950.acf";
	private const string SteamLibraryFoldersFileName = "libraryfolders.vdf";
	private const string SteamVdfPathKey = "path";
	private const string SteamVdfInstallDirKey = "installdir";
	private static readonly string SteamRlRelativePath = Path.Combine(
		SteamAppsDirName,
		SteamCommonDirName,
		SteamRlDirName);

	[JsonConstructor]
	private InstallSteam(string? installDir)
	{
		InstallDir = installDir;
		IsValid = InstallDirIsValid(installDir);
	}

	public InstallSteam()
	{
		InstallDir = null;
		IsValid = false;
	}

	public override void AutoDetectInstallDir()
	{
		if (TryFindFromManifests() || TryFindFromClassicPaths())
		{
			Log.Write(Log.Level.Info, $"{Log.Green}RL Steam dir detected: {Log.Blue}\"{InstallDir}\"{Log.Reset}");
		}
		else
		{
			Log.Write(Log.Level.Warning, "RL Steam dir not detected");
		}
	}

	private bool TryFindFromManifests()
	{
		foreach (string steamRoot in GetRootCandidates())
		{
			foreach (string libraryRoot in GetLibraryCandidates(steamRoot))
			{
				string manifestFile = Path.Combine(
					libraryRoot,
					SteamAppsDirName,
					SteamManifestFileName);
				if (TryFindFromManifest(libraryRoot, manifestFile))
				{
					return true;
				}
			}
		}
		return false;
	}

	private static IEnumerable<string> GetRootCandidates()
	{
		if (!string.IsNullOrWhiteSpace(ProgramFilesX86Dir))
		{
			yield return Path.Combine(ProgramFilesX86Dir, SteamDirName);
		}
		if (!string.IsNullOrWhiteSpace(ProgramFilesDir))
		{
			yield return Path.Combine(ProgramFilesDir, SteamDirName);
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
			yield return Path.Combine(driveRootDir, SteamDirName);
			yield return Path.Combine(driveRootDir, SteamLibraryDirName);
			yield return Path.Combine(driveRootDir, GamesDirName, SteamDirName);
			yield return Path.Combine(driveRootDir, GamesDirName, SteamLibraryDirName);
		}
	}

	private static IEnumerable<string> GetLibraryCandidates(string steamRoot)
	{
		yield return steamRoot;

		string libraryFoldersFile = Path.Combine(
			steamRoot,
			SteamAppsDirName,
			SteamLibraryFoldersFileName);

		string[] libraryFolders;
		try
		{
			libraryFolders = VdfFile.ReadValues(libraryFoldersFile, SteamVdfPathKey);
		}
		catch (Exception exception) when (exception
			is IOException
			or UnauthorizedAccessException
			or NotSupportedException
			or System.Security.SecurityException)
		{
			yield break;
		}
		foreach (string libraryFolder in libraryFolders)
		{
			yield return libraryFolder;
		}
	}

	private bool TryFindFromManifest(string libraryRoot, string manifestFile)
	{
		string installDirName;
		try
		{
			installDirName = VdfFile.ReadValue(
				manifestFile,
				SteamVdfInstallDirKey) ?? SteamRlDirName;
		}
		catch (Exception exception) when (exception
			is IOException
			or UnauthorizedAccessException
			or NotSupportedException
			or System.Security.SecurityException)
		{
			return false;
		}
		string installDir = Path.Combine(
			libraryRoot,
			SteamAppsDirName,
			SteamCommonDirName,
			installDirName);
		if (InstallDirIsValid(installDir))
		{
			InstallDir = installDir;
			return true;
		}
		return false;
	}

	private bool TryFindFromClassicPaths()
	{
		foreach (string steamRoot in GetRootCandidates())
		{
			string installDir = Path.Combine(steamRoot, SteamRlRelativePath);

			if (InstallDirIsValid(installDir))
			{
				InstallDir = installDir;
				return true;
			}
		}
		return false;
	}
}
