namespace RlTracker.Core.Config;

internal sealed partial class ConfigFileIni(string filePath)
{
	private const string GlobalSection = "";
	private readonly Dictionary<string, Dictionary<string, string>> _content = [];
	
	public string FilePath { get; } = NormalizeFilePath(filePath);

	public void SetValue(string? sectionName, string key, string value)
	{
		string sectionKey = NormalizeSection(sectionName);
		string pairKey = NormalizeKey(key);
		string pairValue = NormalizeValue(value);

		EnsureSection(sectionKey);
		_content[sectionKey][pairKey] = pairValue;
	}

	public string GetValue(string? sectionName, string key)
	{
		string sectionKey = NormalizeSection(sectionName);
		string pairKey = NormalizeKey(key);

		if (!_content.TryGetValue(sectionKey, out Dictionary<string, string>? section))
			throw new KeyNotFoundException($"Section \"{sectionKey}\" not found in \"{FilePath}\".");
		if (!section.TryGetValue(pairKey, out string? value))
			throw new KeyNotFoundException($"Key \"{pairKey}\" not found in section \"{sectionKey}\" of \"{FilePath}\".");
		return value;
	}

	private void EnsureSection(string sectionKey)
	{
		if (!_content.ContainsKey(sectionKey))
			_content[sectionKey] = [];
	}
}
