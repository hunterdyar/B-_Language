namespace StarParser.Tokenizer;

//wraps a lexer, so it never has to lex multiple times, and lets the parser backtrack between states.
public class LexerState
{
	private LexerWrapper _lexer;
	private int _pos;
	public LexerState(LexerWrapper lexer)
	{
		_lexer = lexer;
	}

	public Token CurrentToken()
	{
		return _lexer.GetToken(_pos);
	}
	public Token NextToken()
	{
		_pos++;
		return _lexer.GetToken(_pos);
	}

	public LexerState Clone()
	{
		return new LexerState(_lexer)
		{
			_pos = this._pos
		};
	}
}