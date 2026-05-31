using System.Text;

namespace FileIni;

public sealed partial class IniFile
{
	public void Write()
	{
		StringBuilder builder = new();

		EnsureDir();
		foreach (string section in _content.Keys)
			BuildSection(builder, section);
		File.WriteAllText(FilePath, builder.ToString());
	}

	private void EnsureDir()
	{
		string? directory = Path.GetDirectoryName(FilePath);
		if (!string.IsNullOrWhiteSpace(directory))
			Directory.CreateDirectory(directory);
	}

	private void BuildSection(StringBuilder builder, string section)
	{
		if (section != GlobalSection)
			builder.Append('[').Append(section).AppendLine("]");
		Dictionary<string, string> values = _content[section];
		foreach (KeyValuePair<string, string> pair in values)
			builder.Append(pair.Key).Append('=').AppendLine(pair.Value);
		builder.AppendLine();
	}
}
