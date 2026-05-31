using GuillaumeAst.Utils;
using GuillaumeAst.FileVdf;

namespace RlTracker.Core;

public sealed class RlInstallSteam(string? installDir) : RlInstall(installDir)
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

	public override void FindInstallDir()
	{
		try
		{
			if (TryFindFromManifests() || TryFindFromClassicPaths())
				Log.PrintGreen($"RL Steam dir found atuomatically: {Log.Blue}\"{InstallDir}.");
			else
				Log.PrintYellow("RL Steam dir not found atuomatically.");
		}
		catch (Exception exception)
		{
			Log.PrintRed($"Exception while searching RL Steam dir: {exception.Message}.");
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
					return true;
			}
		}
		return false;
	}

	private bool TryFindFromManifest(string libraryRoot, string manifestFile)
	{
		if (!File.Exists(manifestFile))
			return false;
		string installDirName = ReadVdfValueFromFile(
			manifestFile,
			SteamVdfInstallDirKey) ?? SteamRlDirName;
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

	private static IEnumerable<string> GetRootCandidates()
	{
		if (!string.IsNullOrWhiteSpace(ProgramFilesX86Dir))
			yield return Path.Combine(ProgramFilesX86Dir, SteamDirName);
		if (!string.IsNullOrWhiteSpace(ProgramFilesDir))
			yield return Path.Combine(ProgramFilesDir, SteamDirName);
		foreach (DriveInfo drive in DriveInfo.GetDrives())
		{
			if (!drive.IsReady)
				continue;
			yield return Path.Combine(drive.RootDirectory.FullName, SteamDirName);
			yield return Path.Combine(drive.RootDirectory.FullName, SteamLibraryDirName);
			yield return Path.Combine(drive.RootDirectory.FullName, GamesDirName, SteamDirName);
			yield return Path.Combine(drive.RootDirectory.FullName, GamesDirName, SteamLibraryDirName);
		}
	}

	private static IEnumerable<string> GetLibraryCandidates(string steamRoot)
	{
		yield return steamRoot;

		string libraryFoldersFile = Path.Combine(
			steamRoot,
			SteamAppsDirName,
			SteamLibraryFoldersFileName);

		if (!File.Exists(libraryFoldersFile))
			yield break;
		foreach (string line in File.ReadLines(libraryFoldersFile))
		{
			if (TryReadVdfValue(line, SteamVdfPathKey, out string path))
				yield return path;
		}
	}
}
