namespace FileIni;

public sealed partial class IniFile
{
	public void Read()
	{
		string? currentSection = null;
		int lineNumber = 0;

		_content.Clear();
		foreach (string rawLine in File.ReadLines(FilePath))
		{
			string line = rawLine.Trim();

			lineNumber++;
			if (line.Length == 0 || line.StartsWith(';') || line.StartsWith('#'))
				continue;
			else if (line.StartsWith('['))
				ReadSection(line, out currentSection, lineNumber);
			else
				ReadKeyValue(line, currentSection, lineNumber);
		}
	}

	private void ReadSection(string line, out string sectionName, int lineNumber)
	{
		if (!line.EndsWith(']'))
			throw new FormatException($"Missing ']' at line {lineNumber} of \"{FilePath}\".");
		sectionName = line[1..^1].Trim();
		if (sectionName.Length == 0)
			throw new FormatException($"Empty section name at line {lineNumber} of \"{FilePath}\".");
		try
		{
			sectionName = NormalizeSection(sectionName);
			EnsureSection(sectionName);
		}
		catch (ArgumentException exception)
		{
			throw new FormatException(
				$"Invalid section at line {lineNumber} of \"{FilePath}\": {exception.Message}",
				exception
			);
		}
	}

	private void ReadKeyValue(string line, string? sectionName, int lineNumber)
	{
		int separator = line.IndexOf('=');
		string key;
		string value;

		if (separator < 0)
			throw new FormatException($"Missing '=' at line {lineNumber} of \"{FilePath}\".");
		key = line[..separator];
		value = line[(separator + 1)..];
		try
		{
			Set(sectionName, key, value);
		}
		catch (ArgumentException exception)
		{
			throw new FormatException(
				$"Invalid key/value at line {lineNumber} of \"{FilePath}\": {exception.Message}",
				exception
			);
		}
	}
}
