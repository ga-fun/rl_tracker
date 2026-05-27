namespace FileIni;

public sealed partial class FileIni
{
	private static string NormalizeFilePath(string filePath)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
		string normalized = filePath.Trim();

		if (normalized.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
			throw new ArgumentException("File path contains invalid characters.");
		return normalized;
	}

	private static string NormalizeSection(string? value)
	{
		string normalized;

		if (value == null)
			return GlobalSection;
		normalized = value.Trim();
		ArgumentException.ThrowIfNullOrWhiteSpace(normalized);
		RejectLineBreaks("Section", normalized);
		return normalized;
	}

	private static string NormalizeKey(string value)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(value);
		string normalized = value.Trim();

		RejectLineBreaks("Key", normalized);
		if (normalized.Contains('='))
			throw new ArgumentException("Key must not contain '='.");
		if (normalized.StartsWith(';') || normalized.StartsWith('#') || normalized.StartsWith('['))
			throw new ArgumentException("Key must not start with ';', '#' or '['.");
		return normalized;
	}

	private static string NormalizeValue(string value)
	{
		ArgumentNullException.ThrowIfNull(value);
		string normalized = value.Trim();

		RejectLineBreaks("Value", normalized);
		return normalized;
	}

	private static void RejectLineBreaks(string argName, string value)
	{
		if (value.Contains('\n') || value.Contains('\r'))
			throw new ArgumentException($"{argName} must not contain line breaks.");
	}
}
