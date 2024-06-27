
using BMinus.AST;
using StarParser.Tokenizer;

namespace StarParser.Parser.Parselets;

public class PrefixOperatorParselet : IPrefixParselet
{
	private readonly int _bindingPower;

	public PrefixOperatorParselet(int bindingPower)
	{
		_bindingPower = bindingPower;
	}

	public Statement Parse(Parser parser, Token token)
	{
		var right = parser.ParseStatement(_bindingPower) as Expression;
		return PrefixOp.GetPrefixOp(right, token.Literal);
		// return new PrefixExpression(token.TokenType, right, token.Location);
	}

	public int GetBindingPower()
	{
		return _bindingPower;
	}
}