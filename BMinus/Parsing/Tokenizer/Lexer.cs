using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace BMinus.Tokenizer;

public class Lexer
{
	private int _pos;
	private List<Token> _tokenBuffer = new List<Token>();
	private readonly string _source;
	private TokenState _state = TokenState.Entry;
	private Token EOSToken => new Token(TokenType.End, "");
	private const char StringToken = '\"';
	private const char CharToken = '\'';
	private (string, TokenType)[] _keywords = new[]
	{
		("auto", TokenType.VarDeclKeyword),
		("var", TokenType.VarDeclKeyword),
		("extern", TokenType.ExternKeyword),//okay so it's extrn not extern but i kept mistyping it so...
		("extrn", TokenType.ExternKeyword),
		("global", TokenType.ExternKeyword),
		("if",TokenType.IfKeyword),
		("else", TokenType.ElseKeyword),
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
		('/',TokenType.Divide),
		('?',TokenType.QuestionMark),//for ternary
		('%',TokenType.Mod),
		('|',TokenType.Or)
	}.Select< (char, TokenType),
	(Func<char,bool>, TokenType)>(x => ((char c)=> { return (x.Item1 == c); }, x.Item2)).ToList();

	private ((TokenType, TokenType), TokenType)[] _merges = new[]
	{
		((TokenType.Assignment, TokenType.Assignment), TokenType.Equals),
		((TokenType.Bang,TokenType.Assignment),        TokenType.NotEquals),//todo: is this not working?
		((TokenType.GreaterThan, TokenType.Assignment), TokenType.GreaterThanEqual),
		((TokenType.LessThan, TokenType.Assignment), TokenType.LessThanEqual),
		((TokenType.Plus, TokenType.Plus), TokenType.Increment),
		((TokenType.Minus, TokenType.Minus), TokenType.Decrement),
		((TokenType.Plus, TokenType.Assignment), TokenType.AssignmentPlus),
		((TokenType.Minus, TokenType.Assignment), TokenType.AssignmentMinus),
		((TokenType.GreaterThan, TokenType.GreaterThan), TokenType.ShiftRight),
		((TokenType.LessThan, TokenType.LessThan), TokenType.ShiftLeft),

	};

	private void ParseOneOrMore()
	{
		var tokenAdded = false;
		while (!tokenAdded)
		{
			if (_pos >= _source.Length)
			{
				_state = TokenState.Complete;
				break;
			}
			EatWhitespace();
			if (_pos >= _source.Length)
			{
				_state = TokenState.Complete;
				break;
			}

			if (EatSingleCharacters())
			{
				tokenAdded = true;
				continue;
			}

			if (_pos >= _source.Length)
			{
				_state = TokenState.Complete;
				break;
			}

			if (EatIdentifier())
			{
				tokenAdded = true;
				continue;
			}

			if (_pos >= _source.Length)
			{
				_state = TokenState.Complete;
				break;
			}

			if (EatString(StringToken, TokenType.String))
			{
				tokenAdded = true;
				continue;
			}

			if (_pos >= _source.Length)
			{
				_state = TokenState.Complete;
				break;
			}

			if (EatString(CharToken, TokenType.CharLiteral))
			{
				tokenAdded = true;
				continue;
			}

			if (_pos >= _source.Length)
			{
				_state = TokenState.Complete;
				break;
			}

			if (EatInteger())
			{
				tokenAdded = true;
			}

			if (_state == TokenState.Error || _state == TokenState.Complete)
			{
				break;
			}
		}
	}

	public Token NextToken()
	{
		ParseOneOrMore();
		if (_state == TokenState.Complete && _tokenBuffer.Count == 0)
		{
			return EOSToken;
		}
		MergePairTokens();
		if (_tokenBuffer.Count > 0)
		{
			var t = _tokenBuffer[0];
			_tokenBuffer.RemoveAt(0);
			return t;
		}
		else
		{
			//todo: could be in error state.
			return EOSToken;
		}
	}
	
	public Lexer(string source)
	{
		this._source = source;
		this._pos = 0;
		if (string.IsNullOrEmpty(source))
		{
			throw new LexerException("Empty source string to lex.");
		}
	}

	private void MergePairTokens()
	{
		if (_tokenBuffer.Count == 0)
		{
			return;
		}
		var a = _tokenBuffer[0];
		foreach (var merge in _merges)
		{
			if (a.TokenType == merge.Item1.Item1)
			{
				ParseOneOrMore();
				if (_tokenBuffer.Count > 2)
				{
					var b = _tokenBuffer[1];
					if (b.TokenType == merge.Item1.Item2)
					{
						_tokenBuffer.RemoveAt(1);
						_tokenBuffer[0] = new Token(merge.Item2, a.Literal + b.Literal);
						//MergePairTokens();//Keep merging until we cannot merge anymore.
					}
				}
			}
		}
	}

	private bool EatIdentifier()
	{
		var first = _source[_pos];
		if (!char.IsLetter(first))
		{
			return false;
		}
		else
		{
			_state = TokenState.Identifier;
			int start = _pos;
			int length = 1;

			Advance();
			if (_state == TokenState.Complete)
			{
				//a single-letter identifier that ends the input stream
				var ft = new Token(TokenType.Identifier, first.ToString());
				_tokenBuffer.Add(ft);
				return true;
			}
			char c = _source[_pos];
			while (char.IsLetter(c) || char.IsDigit(c) || c == '_' || c == '.')//page 3 of btut.pdf, dot's are allowed in variable names.
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
				return false;
			}

			if (Peek() == ':')
			{
				Consume(':');
				var l = new Token(TokenType.Label, id);
				_tokenBuffer.Add(l);
				_state = TokenState.Entry;
				return true;
			}
			var t = new Token(TokenType.Identifier, id);
			_tokenBuffer.Add(t);
			_state = TokenState.Entry;
			return true;
		}
	}

	private bool CheckIdentifierIsKeyword(string id)
	{
		foreach (var pair in _keywords)
		{
			if (id == pair.Item1)
			{
				var t = new Token(pair.Item2, id);
				_tokenBuffer.Add(t);
				return true;
			}
		}

		return false;
	}

	private bool EatString(char enclosing, TokenType tt)
	{
		var first = _source[_pos];
		if (first != enclosing)
		{
			return false;
		}
		else
		{
			Consume(enclosing);//past the first '
			_state = TokenState.String;
			int start = _pos;
			int length = 1;
			Advance();
			char c = _source[_pos];
			bool escape = true;
			while (c != enclosing || escape)
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

			Consume(enclosing);
			var id = _source.Substring(start, length);
			var t = new Token(tt, id);
			_tokenBuffer.Add(t);
			_state = TokenState.Entry;
			return true;
		}
	}
	private bool EatInteger()
	{
		var first = _source[_pos];
		if (!char.IsDigit(first))
		{
			return false;
		}
		else
		{
			_state = TokenState.Integer;
			int start = _pos;
			int length = 1;
			Advance();
			if (_state == TokenState.Complete)
			{
				//we forgot the ; and the stream ended, but thhe error should be that; this part should still parse.
				var tf = new Token(TokenType.IntLiteral, first.ToString());
				_tokenBuffer.Add(tf);
				return true;
			}
			char c = _source[_pos];
			if (first == '0' && c == 'x' || c == 'X')
			{
				Advance();//past x
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
				_tokenBuffer.Add(ht);
				_state = TokenState.Entry;
				return true;
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
			_tokenBuffer.Add(t);
			_state = TokenState.Entry;
			return true;
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

	private bool EatSingleCharacters()
	{
		foreach (var r in _singleCharRules)
		{
			if (r.Item1(_source[_pos]))
			{
				Token t = new Token(r.Item2, _source[_pos].ToString());
				_tokenBuffer.Add(t);
				Advance();
				return true;
			}
		}

		return false;
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