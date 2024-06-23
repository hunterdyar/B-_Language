using BMinus.AST;
using Superpower;
using Superpower.Parsers;

namespace BMinus.Parser;

public class LiteralParsers
{
	public static readonly TokenListParser<BMToken, Expression> IntegerLiteral =
    		from text in Token.EqualTo(BMToken.IntLiteral)
    		select (Expression)new WordLiteral(text,int.Parse(text.Span.ToString()));
	
	public static readonly TokenListParser<BMToken, Expression> LiteralParser =
    		from lit in IntegerLiteral
    		select lit;
}