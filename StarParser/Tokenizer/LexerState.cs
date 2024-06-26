using System.Reflection.Metadata.Ecma335;

namespace StarParser.Tokenizer;

//wraps a lexer, so it never has to lex multiple times, and lets the parser backtrack between states.
public class LexerState
{
	private LexerWrapper _lexer;
	private int _pos;
	public LexerState(LexerWrapper lexer, int pos = 0)
	{
		_lexer = lexer;
		pos = 0;
	}
	public Token NextToken()
	{
		var t = _lexer.GetToken(_pos);
		_pos++;
		return t;
	}

	public Token Peek()
	{
		return _lexer.GetToken(_pos);
	}

	public void Advance()
	{
		_pos++;
	}

	public LexerState Clone()
	{
		return new LexerState(_lexer)
		{
			_pos = this._pos
		};
	}

	public void From(LexerState lexerState)
	{
		_pos = lexerState._pos;
	}

	public bool Consume(TokenType t)
	{
		if (Peek().TokenType == t)
		{
			Advance();
			return true;
		}

		return false;
	}
}