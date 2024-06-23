using System.Xml.Xsl;
using BMinus.AST;
using Superpower;
using Superpower.Parsers;
using Identifier = BMinus.AST.Identifier;

namespace BMinus.Parser;

public class ExpressionParsers
{
	public static bool IsOperator(BMToken token)
	{
		switch (token)
		{
			case BMToken.Plus:
			case BMToken.Minus:
			case BMToken.Times:
			case BMToken.Divide:
			case BMToken.Assign:
				return true;
		}

		return false;
	}
	public static readonly TokenListParser<BMToken, Expression> IdentifierParser =
		from id in Token.EqualTo(BMToken.Identifier)
		select (Expression)new Identifier(id);
	
	public static readonly TokenListParser<BMToken, Expression> BinaryExpressionParser = 
		from left in ExpressionParser
		from op in Token.Matching<BMToken>(IsOperator,"Is Operator")
		from right in ExpressionParser
		select (Expression)BinOp.GetBinaryOp(left, op, right);
	
	public static readonly TokenListParser<BMToken, Expression> ExpressionParser =
		from expr in IdentifierParser.Or(LiteralParsers.LiteralParser).Or(BinaryExpressionParser)
             select expr;
}