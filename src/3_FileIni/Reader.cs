namespace GuillaumeAst.FileIni;

internal static class Reader
{
	private sealed class State(string filePath)
	{
		internal IniFile DestinationFile { get; } = new(filePath);
		internal string? CurrentSection { get; set; } = null;
		internal int LineNumber { get; set; } = 0;
	}

	internal static IniFile Read(string filePath)
	{
		State state = new(filePath);

		foreach (string rawLine in File.ReadLines(state.DestinationFile.FilePath))
		{
			state.LineNumber++;
			string line = rawLine.Trim();
			if (line.Length > 0 && !line.StartsWith(';') && !line.StartsWith('#'))
			{
				if (line.StartsWith('['))
				{
					ReadSection(state, line);
				}
				else
				{
					ReadKeyValue(state, line);
				}
			}
		}
		return state.DestinationFile;
	}

	private static void ReadSection(State state, string line)
	{
		if (!line.EndsWith(']'))
		{
			throw new FormatException($"Missing ']' at line {state.LineNumber} of \"{state.DestinationFile.FilePath}\"");
		}
		string sectionName = line[1..^1].Trim();
		if (sectionName.Length == 0)
		{
			throw new FormatException($"Empty section name at line {state.LineNumber} of \"{state.DestinationFile.FilePath}\"");
		}
		try
		{
			state.DestinationFile.EnsureSection(sectionName);
			state.CurrentSection = sectionName;
		}
		catch (ArgumentException exception)
		{
			throw new FormatException(
				$"Invalid section at line {state.LineNumber} of \"{state.DestinationFile.FilePath}\": {exception.Message}",
				exception
			);
		}
	}

	private static void ReadKeyValue(State state, string line)
	{
		int separator = line.IndexOf('=');
		string key;
		string value;

		if (separator < 0)
		{
			throw new FormatException($"Missing '=' at line {state.LineNumber} of \"{state.DestinationFile.FilePath}\"");
		}
		key = line[..separator];
		value = line[(separator + 1)..];
		try
		{
			state.DestinationFile.Set(state.CurrentSection, key, value);
		}
		catch (ArgumentException exception)
		{
			throw new FormatException(
				$"Invalid key/value at line {state.LineNumber} of \"{state.DestinationFile.FilePath}\": {exception.Message}",
				exception
			);
		}
	}
}
