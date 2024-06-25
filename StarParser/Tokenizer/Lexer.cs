using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace StarParser.Tokenizer;

public class Lexer
{
	private int _pos;
	public List<Token> Tokens => _tokens;
	private List<Token> _tokens = new List<Token>();
	private readonly string _source;
	private TokenState _state = TokenState.Entry;
	private string _buffer;

	private (string, TokenType)[] _keywords = new[]
	{
		("auto", TokenType.VarDeclKeyword),
		("var", TokenType.VarDeclKeyword),
		("extern", TokenType.ExternKeyword),
		("global", TokenType.ExternKeyword),
		("if",TokenType.IfKeyword),
		("while",TokenType.WhileKeyword),
		("switch",TokenType.SwitchKeyword),
		("case", TokenType.CaseKeyword),
		("default", TokenType.DefaultKeyword),
		("break", TokenType.BreakKeyword),
		("goto",TokenType.GoToKeyword),
		("return", TokenType.ReturnKeyword),
	};

	private List<(Func<char,bool>, TokenType)> _singleCharRules = new List<(char, TokenType)>()
	{
		('(', TokenType.LParen),
		(')',TokenType.RParen),
		('{',TokenType.LBrace),
		('}',TokenType.RBrace),
		('[',TokenType.LBracket),
		(']',TokenType.RBracket),
		(',',TokenType.Comma),
		(';',TokenType.EndStatement),
		('~', TokenType.Tilde),
		('!',TokenType.Bang),
		('<',TokenType.LessThan),
		('>',TokenType.GreaterThan),
		('=',TokenType.Assignment),
		(':',TokenType.Colon),
		('+', TokenType.Plus),
		('-', TokenType.Plus),
		('&',TokenType.And),//a & b is bitwise and. &id is adress-of, and && is conditional and?
		('*', TokenType.Asterisk),//a*b is times, *a is indirection (pointer dereference)
		('?',TokenType.QuestionMark),//for ternary
		('%',TokenType.Mod)
	}.Select< (char, TokenType),
	(Func<char,bool>, TokenType)>(x => ((char c)=> { return (x.Item1 == c); }, x.Item2)).ToList();

	private ((TokenType, TokenType), TokenType)[] _merges = new[]
	{
		((TokenType.Assignment, TokenType.Assignment), TokenType.Equals),
		((TokenType.Bang,TokenType.Assignment),        TokenType.NotEquals),
		((TokenType.GreaterThan, TokenType.Assignment), TokenType.GreaterThanEqual),
		((TokenType.LessThan, TokenType.Assignment), TokenType.LessThanEqual),
		((TokenType.Plus, TokenType.Plus), TokenType.Increment),
		((TokenType.Minus, TokenType.Minus), TokenType.Decrement),
		((TokenType.Plus, TokenType.Assignment), TokenType.AssignmentPlus),
		((TokenType.Minus, TokenType.Assignment), TokenType.AssignmentMinus),
		((TokenType.GreaterThan, TokenType.GreaterThan), TokenType.ShiftRight),
		((TokenType.LessThan, TokenType.LessThan), TokenType.ShiftLeft),

	};
	
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
			var current = _source[_pos];
			EatWhitespace();
			if (_pos >= _source.Length)
			{
				_state = TokenState.Complete;
				break;
			}
			EatSingleCharacters();
			if (_pos >= _source.Length)
			{
				_state = TokenState.Complete;
				break;
			}
			EatIdentifier();
			if (_pos >= _source.Length)
			{
				_state = TokenState.Complete;
				break;
			}

			EatString();
			if (_pos >= _source.Length)
			{
				_state = TokenState.Complete;
				break;
			}

			EatInteger();
		}

		MergePairTokens();
	}

	private void MergePairTokens()
	{
		
		for (int i = 1; i < _tokens.Count; i++)
		{
			foreach (var merge in _merges)
			{
				if (_tokens[i].TokenType == merge.Item1.Item2)
				{
					var a = _tokens[i - 1];
					var b = _tokens[i];
					if (a.TokenType == merge.Item1.Item1)
					{
						_tokens.RemoveAt(i);
						_tokens[i - 1] = new Token(merge.Item2, a.Literal + b.Literal);
					}
				}
			}
		}
	}

	private void EatIdentifier()
	{
		var first = _source[_pos];
		if (!char.IsLetter(first))
		{
			return;
		}
		else
		{
			_state = TokenState.Identifier;
			int start = _pos;
			int length = 1;

			Advance();
			char c = _source[_pos];
			while (char.IsLetter(c) || char.IsDigit(c) || c == '_')
			{
				length++;
				Advance();
				//next
				if(_state != TokenState.Identifier) { break;}
				c = _source[_pos];
			}

			var id = _source.Substring(start, length);
			if (CheckIdentifierIsKeyword(id))
			{
				_state = TokenState.Entry;
				return;
			}

			var t = new Token(TokenType.Identifier, id);
			_tokens.Add(t);
			_state = TokenState.Entry;
			return;
		}
	}

	private bool CheckIdentifierIsKeyword(string id)
	{
		foreach (var pair in _keywords)
		{
			if (id == pair.Item1)
			{
				var t = new Token(pair.Item2, id);
				_tokens.Add(t);
				return true;
			}
		}

		return false;
	}

	private void EatString()
	{
		var first = _source[_pos];
		if (first != '\"')
		{
			return;
		}
		else
		{
			Consume('"');//past the first "
			_state = TokenState.String;
			int start = _pos;
			int length = 1;
			Advance();
			char c = _source[_pos];
			bool escape = true;
			while (c != '"' || escape)
			{
				if (c == '\\')
				{
					escape = true;
				}
				else
				{
					escape = false;
				}
				
				length++;
				Advance();
				//next
				if (_state != TokenState.String)
				{
					break;
				}

				c = _source[_pos];
			}

			Consume('"');
			var id = _source.Substring(start, length);
			var t = new Token(TokenType.String, id);
			_tokens.Add(t);
			_state = TokenState.Entry;
			return;
		}
	}
	private void EatInteger()
	{
		var first = _source[_pos];
		if (!char.IsDigit(first))
		{
			return;
		}
		else
		{
			_state = TokenState.Integer;
			int start = _pos;
			int length = 1;
			Advance();
			char c = _source[_pos];
			if (first == '0' && c == 'x' || c == 'X')
			{
				Advance();//past 0
				length++;
				_state = TokenState.HexInteger;
				c = _source[_pos];
				while (char.IsDigit(c) || char.IsBetween(c,'a','f') || char.IsBetween(c,'A','F'))
				{
					length++;
					Advance();
					//next
					if (_state != TokenState.HexInteger)
					{
						break;
					}

					c = _source[_pos];
				}

				var hexID = _source.Substring(start, length);
				var ht = new Token(TokenType.HexLiteral, hexID);
				_tokens.Add(ht);
				_state = TokenState.Entry;
				return;
			}
			while (char.IsDigit(c) || c == '.')
			{
				length++;
				Advance();
				//next
				if (_state != TokenState.Integer && _state!= TokenState.HexInteger)
				{
					break;
				}

				c = _source[_pos];
			}

			var id = _source.Substring(start, length);
			var t = new Token(TokenType.IntLiteral, id);
			_tokens.Add(t);
			_state = TokenState.Entry;
			return;
		}
	}

	private void Consume(char c)
	{
		if (_source[_pos] == c)
		{
			Advance();
			return;
		}

		throw new Exception("fuck!");
	}
	private char Peek()
	{
		if (_pos < _source.Length-1)
		{
			return _source[_pos + 1];
		}

		return char.MinValue;
	}

	private void EatSingleCharacters()
	{
		foreach (var r in _singleCharRules)
		{
			if (r.Item1(_source[_pos]))
			{
				Token t = new Token(r.Item2, _source[_pos].ToString());
				_tokens.Add(t);
				Advance();
				break;
			}
		}
	}

	private void EatWhitespace()
	{
		var c = _source[_pos];
		while (c == ' ' || c == '\n' || c == '\r' || c == '\t')
		{
			Advance();
			if (_state == TokenState.Complete)
			{
				return;
			}
			c = _source[_pos];
		}
	}

	private void Advance()
	{
		_pos++;
		if (_pos >= _source.Length)
		{
			_state = TokenState.Complete;
		}
	}
}