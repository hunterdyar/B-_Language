using Superpower.Model;

namespace StarParser.Tokenizer;

public class LexerWrapper
{
	private Lexer _lexer;
	private List<Token> _tokens = new List<Token>();
	
	public LexerWrapper(Lexer lexer)
	{
		_lexer = lexer;
	}

	public Token GetToken(int pos)
	{
		while (pos >= _tokens.Count)
		{
			var t = _lexer.NextToken();
			_tokens.Add(t);
			if (t.TokenType == TokenType.End)
			{
				break;
			}
		}

		return _tokens[pos];
	}

	public LexerState GetLexerState()
	{
		return new LexerState(this);
	}
}