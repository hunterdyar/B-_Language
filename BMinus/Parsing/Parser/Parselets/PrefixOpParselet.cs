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
		return PrefixOp.GetPrefixOp(rightExpr, token.Literal);
	}
}