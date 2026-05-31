namespace GuillaumeAst.FileIni;

public sealed class IniFile(string filePath)
{
	public string FilePath { get; } = Normalizer.NormalizeFilePath(filePath);
	internal readonly Dictionary<string, Dictionary<string, string>> Content = [];

	public static IniFile Read(string filePath)
	{
		return Reader.Read(filePath);
	}

	public void Write()
	{
		Writer.Write(this);
	}

	public void Set(string? sectionName, string key, string value)
	{
		string sectionKey = Normalizer.NormalizeSection(sectionName);
		string pairKey = Normalizer.NormalizeKey(key);
		string pairValue = Normalizer.NormalizeValue(value);

		EnsureSection(sectionKey);
		Content[sectionKey][pairKey] = pairValue;
	}

	public string Get(string? sectionName, string key)
	{
		string sectionKey = Normalizer.NormalizeSection(sectionName);
		string pairKey = Normalizer.NormalizeKey(key);

		if (!Content.TryGetValue(sectionKey, out Dictionary<string, string>? section))
		{
			throw new KeyNotFoundException($"Section \"{sectionKey}\" not found in \"{FilePath}\".");
		}
		if (!section.TryGetValue(pairKey, out string? value))
		{
			throw new KeyNotFoundException($"Key \"{pairKey}\" not found in section \"{sectionKey}\" of \"{FilePath}\".");
		}
		return value;
	}

	internal void EnsureSection(string sectionName)
	{
		string sectionKey = Normalizer.NormalizeSection(sectionName);

		if (!Content.ContainsKey(sectionKey))
		{
			Content[sectionKey] = [];
		}
	}
}
