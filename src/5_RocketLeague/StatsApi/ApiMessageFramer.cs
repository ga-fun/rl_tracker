using System.Text;
using GuillaumeAst.Utils;

namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class ApiMessageFramer
{
	private readonly Decoder _decoder = Encoding.UTF8.GetDecoder();
	private readonly StringBuilder _buffer = new();

	public List<string> GetApiMessages(byte[] bytes)
	{
		AppendBytes(bytes);
		return ExtractMessages();
	}

	private void AppendBytes(byte[] bytes)
	{
		char[] chars = new char[Encoding.UTF8.GetMaxCharCount(bytes.Length)];
		int count = _decoder.GetChars(bytes, 0, bytes.Length, chars, 0);

		_buffer.Append(chars, 0, count);
	}

	private List<string> ExtractMessages()
	{
		List<string> messages = [];
		int start = FindObjectStart(0);
		int removeLength = start < 0 ? _buffer.Length : start;

		while (start >= 0 && TryFindObjectEnd(start, out int end))
		{
			messages.Add(_buffer.ToString(start, end - start + 1));
			removeLength = end + 1;
			start = FindObjectStart(removeLength);
		}
		if (start < 0)
		{
			removeLength = _buffer.Length;
		}
		RemoveProcessedChars(removeLength);
		return messages;
	}

	private void RemoveProcessedChars(int length)
	{
		if (length > 0)
		{
			_buffer.Remove(0, length);
		}
	}

	private int FindObjectStart(int start)
	{
		for (int index = start; index < _buffer.Length; index++)
		{
			if (_buffer[index] == '{')
			{
				return index;
			}
		}
		return -1;
	}

	private bool TryFindObjectEnd(int start, out int end)
	{
		int depth = 0;
		bool inString = false;
		bool escaped = false;

		for (int index = start; index < _buffer.Length; index++)
		{
			UpdateJsonState(_buffer[index], ref inString, ref escaped, ref depth);
			if (!inString && depth == 0)
			{
				end = index;
				return true;
			}
		}
		end = -1;
		return false;
	}

	private static void UpdateJsonState(
		char character,
		ref bool inString,
		ref bool escaped,
		ref int depth)
	{
		if (escaped)
		{
			escaped = false;
			return;
		}
		if (character == '\\' && inString)
		{
			escaped = true;
			return;
		}
		if (character == '"')
		{
			inString = !inString;
			return;
		}
		UpdateDepth(character, inString, ref depth);
	}

	private static void UpdateDepth(char character, bool inString, ref int depth)
	{
		if (inString)
		{
			return;
		}
		if (character == '{')
		{
			depth++;
		}
		else if (character == '}')
		{
			depth--;
		}
	}
}
