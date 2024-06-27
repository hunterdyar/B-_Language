
using BMinus.AST;
using StarParser.Tokenizer;

namespace StarParser.Parser.Parselets;

public class PostfixOperatorParselet : IInfixParselet
{
	private readonly int _bindingPower;

	public PostfixOperatorParselet(int bindingPower)
	{
		_bindingPower = bindingPower;
	}

	public Statement Parse(Parser parser, Statement left, Token token)
	{
		if (token.TokenType == TokenType.Increment)
		{
		//	return new IncrementExpression(left, token.TokenType, token.Location);
		}
		else if (token.TokenType == TokenType.Decrement)
		{
		//	return new DecrementExpression(left, token.TokenType, token.Location);
		}

		throw new ParseException($"Unable to parse Postfix operator {token.TokenType}");
	}

	public int GetBindingPower()
	{
		return _bindingPower;
	}
}