namespace FileIni;

public sealed partial class IniFile(string filePath)
{
	private const string GlobalSection = "";
	private readonly Dictionary<string, Dictionary<string, string>> _content = [];
	
	public string FilePath { get; } = NormalizeFilePath(filePath);

	public void Set(string? sectionName, string key, string value)
	{
		string sectionKey = NormalizeSection(sectionName);
		string pairKey = NormalizeKey(key);
		string pairValue = NormalizeValue(value);

		EnsureSection(sectionKey);
		_content[sectionKey][pairKey] = pairValue;
	}

	public string Get(string? sectionName, string key)
	{
		string sectionKey = NormalizeSection(sectionName);
		string pairKey = NormalizeKey(key);

		if (!_content.TryGet(sectionKey, out Dictionary<string, string>? section))
			throw new KeyNotFoundException($"Section \"{sectionKey}\" not found in \"{FilePath}\".");
		if (!section.TryGet(pairKey, out string? value))
			throw new KeyNotFoundException($"Key \"{pairKey}\" not found in section \"{sectionKey}\" of \"{FilePath}\".");
		return value;
	}

	private void EnsureSection(string sectionKey)
	{
		if (!_content.ContainsKey(sectionKey))
			_content[sectionKey] = [];
	}
}
