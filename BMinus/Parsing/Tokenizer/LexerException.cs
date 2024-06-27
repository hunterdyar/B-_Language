using System.Runtime.CompilerServices;

namespace BMinus.Tokenizer;

public class LexerException: Exception
{
	public LexerException(string message) : base(message)
	{
	}
}