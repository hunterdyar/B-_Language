using BMinus.AST;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public class TernaryParselet : IInfixParselet
{
	public Statement Parse(Parser parser, Statement left, Token token)
	{
		if (!(left is Expression condition))
		{
			throw new ParseException("Ternary Condition must be exception");

		}
		var consequence = parser.ParseStatement();
		if (!(consequence is Expression consExpr))
		{
			throw new ParseException("Ternary Consequence must be exception");
		}
		parser.Consume(TokenType.Colon);
		var alternative = parser.ParseStatement();
		if (!(alternative is Expression altExpr))
		{
			throw new ParseException("Ternary Alternative must be exception");
		}

		return new TernaryOp(condition, consExpr, altExpr);
	}

	public int GetBindingPower()
	{
		return BindingPower.Ternery;
	}
}