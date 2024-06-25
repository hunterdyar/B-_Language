using System.Net;

namespace StarParser.Tokenizer;

public class Lexer
{
	private int _pos;
	public List<Token> Tokens => _tokens;
	private List<Token> _tokens = new List<Token>();
	private readonly string _source;
	private TokenState _state = TokenState.Entry;
	private string _buffer;
	
	public Lexer(string source)
	{
		_buffer = "";
		this._source = source;
		this._pos = 0;
		if (string.IsNullOrEmpty(source))
		{
			throw new LexerException("Empty source string to lex.");
		}
		
		while (_pos < source.Length && _state != TokenState.Error && _state!= TokenState.Complete)
		{
			TokenizeNext();
		}
	}

	private void TokenizeNext()
	{
		var c = Current();
		if (_state == TokenState.Entry)
		{
			switch (c)
			{
				case ' ':
				case '\n':
				case '\t':
					Advance();
					break;
				case '{':
					CreateAndAddToken(TokenType.LBrace);
					break;
				case '}':
					CreateAndAddToken(TokenType.RBrace);
					break;
				case '(':
					CreateAndAddToken(TokenType.LParen);
					break;
				case ')':
					CreateAndAddToken(TokenType.RParen);
					break;
				case '[':
					CreateAndAddToken(TokenType.LBracket);
					break;
				case ']':
					CreateAndAddToken(TokenType.RBracket);
					break;
				case ',':
					CreateAndAddToken(TokenType.Comma);
					break;
				case ';':
					CreateAndAddToken(TokenType.EndStatement);
					break;
				case '0':
					_state = TokenState.Integer;
					_buffer += '0';
					break;
			}
		}else if (_state == TokenState.Integer)
		{
			if (char.IsDigit(c) || c == '.')
			{
				_buffer += c;
				Advance();
				return;
			}
			Token t = new Token(TokenType.IntLiteral, _buffer);
			_tokens.Add(t);
			_buffer = "";
			//don't advance the current character, it's not a digit, we need to tokenize it still.
			return;
		}
	}

	private void CreateAndAddToken(TokenType tt)
	{
		var token = new Token(tt, Current().ToString());
		//Add Current Position.
		_tokens.Add(token);
		Advance();
	}

	private void CreateAndAddToken(TokenType tt, string literal)
	{
		var token = new Token(tt, literal);
		_tokens.Add(token);
	}

	private void Advance()
	{
		_pos++;
		if (_pos == _source.Length)
		{
			_state = TokenState.Complete;
		}
	}

	private char Current()
	{
		return _source[_pos];
	}
	
	private char Next()
	{
		char c = _source[_pos+1];
		return c;
	}
	
	private char PeekChar()
	{
		if (_pos < _source.Length)
		{
			return _source[_pos + 1];
		}
		
		return ' ';
	}
	
	private void ConsumeChar(char x)
	{
		if (_source[_pos] == x)
		{
			_pos++;
		}
		else
		{
			throw new LexerException($"Can't Consume Character {x}");
		}
	}
}