using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

//a = b = c is parsed as a = (b = c)
public class AssignParselet : IInfixParselet
{
	public Statement Parse(Parser parser, Statement left, Token token)
	{
		var right = parser.ParseStatement(BindingPower.Assignment - 1);
		if (right is not Expression rightExpr)
		{
			throw new ParseException($"Cannot Assign {right} to {left}");
		}
		if (left is Identifier name)
		{
			return new Assignment(name, rightExpr);
		}
		
		throw new ParseException($"The left-hand side of an assignment must be an identifier. Have: {left}");
	}

	public int GetBindingPower()
	{
		return BindingPower.Assignment;
	}
}