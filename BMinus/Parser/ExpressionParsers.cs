using BMinus.AST;
using Superpower;
using Superpower.Parsers;
using Identifier = BMinus.AST.Identifier;

namespace BMinus.Parser;

public class ExpressionParsers
{
	
	
	public static TokenListParser<BMToken, Expression> IdentifierParser =
		from id in Token.EqualTo(BMToken.Identifier)
		select (Expression)new Identifier(id);
	
	public static TokenListParser<BMToken, Expression> ExpressionParser =
                                              		from expr in IdentifierParser.Or(LiteralParsers.LiteralParser)
                                              		select expr;
}