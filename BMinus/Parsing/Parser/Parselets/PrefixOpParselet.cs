using BMinus.AST;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public class PrefixOpParselet : IPrefixParselet
{
	public Statement Parse(Parser parser, Token token)
	{
		var right = parser.ParseStatement();
		if (!(right is Expression rightExpr))
		{
			throw new ParseException($"cannot use prefix operator {token.Literal} on {right}");
		}
		switch (token.TokenType)
		{
			case TokenType.Bang:
				return new Bang(rightExpr);
			case TokenType.Minus:
				return new Negate(rightExpr);
			case TokenType.Plus:
				//todo: hmmm
				return rightExpr;
		}

		throw new ParseException($"Bad prefix operator {token.TokenType}.");

	}
}