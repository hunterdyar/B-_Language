using System.Net.Mime;
using BMinus.AST;
using Superpower;
using Superpower.Parsers;

namespace BMinus.Parser;

public class LiteralParsers
{
	public static readonly TokenListParser<BMToken, Expression> IntegerLiteral =
    		from text in Token.EqualTo(BMToken.IntLiteral)
    		select (Expression)new WordLiteral(text,int.Parse(text.Span.ToString()));

	public static readonly TokenListParser<BMToken, Expression> HexLiteral =
		from hex in Token.EqualTo(BMToken.HexLiteral)
		select (Expression)new WordLiteral(hex, 0);//todo
	
	public static readonly TokenListParser<BMToken, Expression> LiteralParser =
    		from lit in IntegerLiteral
    		select lit;
	
}