namespace RlTracker.Core.Config;

internal sealed partial class Config
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

	public static string? FindSteamRlInstallDir()
	{
		try
		{
			return FindSteamRlInstallDirFromManifests()
				?? FindSteamRlInstallDirFromClassicPaths();
		}
		catch
		{
			return null;
		}
	}

	private static string? FindSteamRlInstallDirFromManifests()
	{
		foreach (string steamRoot in GetSteamRootCandidates())
		{
			foreach (string libraryRoot in GetSteamLibraryCandidates(steamRoot))
			{
				string? installDir = FindSteamRlInstallDirFromManifest(libraryRoot);

				if (installDir != null)
					return installDir;
			}
		}
		return null;
	}

	private static string? FindSteamRlInstallDirFromClassicPaths()
	{
		foreach (string steamRoot in GetSteamRootCandidates())
		{
			string installDir = Path.Combine(steamRoot, SteamRlRelativePath);

			if (IsRlInstallDir(installDir))
				return installDir;
		}
		return null;
	}

	private static string? FindSteamRlInstallDirFromManifest(string libraryRoot)
	{
		string manifestFile = Path.Combine(
			libraryRoot,
			SteamAppsDirName,
			SteamManifestFileName);

		if (!File.Exists(manifestFile))
			return null;
		string installDirName = ReadVdfValueFromFile(
			manifestFile,
			SteamVdfInstallDirKey) ?? SteamRlDirName;
		string installDir = Path.Combine(
			libraryRoot,
			SteamAppsDirName,
			SteamCommonDirName,
			installDirName);
		if (IsRlInstallDir(installDir))
			return installDir;
		return null;
	}

	private static IEnumerable<string> GetSteamRootCandidates()
	{
		if (!string.IsNullOrWhiteSpace(ProgramFilesX86))
			yield return Path.Combine(ProgramFilesX86, SteamDirName);
		if (!string.IsNullOrWhiteSpace(ProgramFiles))
			yield return Path.Combine(ProgramFiles, SteamDirName);
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

	private static IEnumerable<string> GetSteamLibraryCandidates(string steamRoot)
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

	private static string? ReadVdfValueFromFile(string filePath, string key)
	{
		foreach (string line in File.ReadLines(filePath))
		{
			if (TryReadVdfValue(line, key, out string value))
				return value;
		}
		return null;
	}

	private static bool TryReadVdfValue(string line, string key, out string value)
	{
		string trimmed = line.Trim();
		string prefix = $"\"{key}\"";
		int start;
		int end;

		value = "";
		if (!trimmed.StartsWith(prefix, StringComparison.Ordinal))
			return false;
		start = trimmed.IndexOf('"', prefix.Length);
		if (start < 0)
			return false;
		end = trimmed.IndexOf('"', start + 1);
		if (end < 0)
			return false;
		value = trimmed[(start + 1)..end].Replace(@"\\", @"\").Trim();
		return value.Length > 0;
	}
}
