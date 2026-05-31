namespace GuillaumeAst.FileVdf;

public static class VdfFile
{
	public static string? ReadValue(string filePath, string key)
	{
		foreach (string value in ReadValues(filePath, key))
		{
			return value;
		}
		return null;
	}

	public static IEnumerable<string> ReadValues(string filePath, string key)
	{
		foreach (string line in File.ReadLines(filePath))
		{
			if (TryReadValue(line, key, out string value))
			{
				yield return value;
			}
		}
	}

	private static bool TryReadValue(string line, string key, out string value)
	{
		string trimmed = line.Trim();
		string prefix = $"\"{key}\"";
		int start;
		int end;

		value = "";
		if (!trimmed.StartsWith(prefix, StringComparison.Ordinal))
		{
			return false;
		}
		start = trimmed.IndexOf('"', prefix.Length);
		if (start < 0)
		{
			return false;
		}
		end = trimmed.IndexOf('"', start + 1);
		if (end < 0)
		{
			return false;
		}
		value = trimmed[(start + 1)..end].Replace(@"\\", @"\").Trim();
		return value.Length > 0;
	}
}
