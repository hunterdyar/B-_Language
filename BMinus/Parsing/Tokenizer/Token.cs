namespace BMinus.Tokenizer;

public class Token
{
	public TokenType TokenType;
	public string Literal;
	//Position

	public Token(TokenType tt, string literal)
	{
		this.TokenType = tt;
		this.Literal = literal;
	}

	public override string ToString()
	{
		if (TokenType == TokenType.Identifier)
		{
			return "Identifier(" + Literal + ")";
		}else if (TokenType == TokenType.IntLiteral)
		{
			return "Int(" + Literal + ")";
		}
		else if (TokenType == TokenType.HexLiteral)
		{
			return "Hex(" + Literal + ")";
		}
		else if (TokenType == TokenType.String)
		{
			return "String(" + Literal + ")";
		}

		return TokenType.ToString();
	}

	public static bool IsInfixOp(TokenType opTokenType)
	{
		switch(opTokenType)
		{
			case TokenType.Plus:
			case TokenType.Minus: 
				case TokenType.Asterisk:
				case TokenType.Mod:
					return true;
		}

		return false;
	}
}