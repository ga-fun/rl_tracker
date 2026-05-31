using System.Text;

namespace GuillaumeAst.FileIni;

internal static class Writer
{
	internal static void Write(IniFile file)
	{
		ArgumentNullException.ThrowIfNull(file);
		StringBuilder builder = new();

		EnsureDir(file.FilePath);
		foreach (var sectionPair in file.Content)
		{
			BuildSection(builder, sectionPair.Key, sectionPair.Value);
		}
		File.WriteAllText(file.FilePath, builder.ToString());
	}

	private static void EnsureDir(string filePath)
	{
		string? directory = Path.GetDirectoryName(filePath);
		if (!string.IsNullOrWhiteSpace(directory))
		{
			Directory.CreateDirectory(directory);
		}
	}

	private static void BuildSection(
		StringBuilder builder,
		string section,
		Dictionary<string, string> values)
	{
		if (section != Normalizer.GlobalSection)
		{
			builder.Append('[').Append(section).AppendLine("]");
		}
		foreach (KeyValuePair<string, string> pair in values)
		{
			builder.Append(pair.Key).Append('=').AppendLine(pair.Value);
		}
		builder.AppendLine();
	}
}
