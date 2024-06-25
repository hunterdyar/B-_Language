using System.Runtime.CompilerServices;

namespace StarParser.Tokenizer;

public class LexerException: Exception
{
	public LexerException(string message) : base(message)
	{
	}
}