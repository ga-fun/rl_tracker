namespace GuillaumeAst.FileIni;

internal static class Normalizer
{
	internal const string GlobalSection = "";

	internal static string NormalizeFilePath(string filePath)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
		string normalized = filePath.Trim();

		if (normalized.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
		{
			throw new ArgumentException("File path contains invalid characters");
		}
		return normalized;
	}

	internal static string NormalizeSection(string? value)
	{
		string normalized;

		if (value == null)
		{
			return GlobalSection;
		}
		ArgumentException.ThrowIfNullOrWhiteSpace(value);
		normalized = value.Trim();
		RejectLineBreaks("Section", normalized);
		return normalized;
	}

	internal static string NormalizeKey(string value)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(value);
		string normalized = value.Trim();

		RejectLineBreaks("Key", normalized);
		if (normalized.Contains('='))
		{
			throw new ArgumentException("Key must not contain '='");
		}
		if (normalized.StartsWith(';') || normalized.StartsWith('#') || normalized.StartsWith('['))
		{
			throw new ArgumentException("Key must not start with ';', '#' or '['");
		}
		return normalized;
	}

	internal static string NormalizeValue(string value)
	{
		ArgumentNullException.ThrowIfNull(value);
		string normalized = value.Trim();

		RejectLineBreaks("Value", normalized);
		return normalized;
	}

	private static void RejectLineBreaks(string argName, string value)
	{
		if (value.Contains('\n') || value.Contains('\r'))
		{
			throw new ArgumentException($"{argName} must not contain line breaks");
		}
	}
}
