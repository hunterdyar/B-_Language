
using BMinus.AST;
using StarParser.Tokenizer;

namespace StarParser.Parser.Parselets;

public class BinaryOperatorParselet : IInfixParselet
{
	private readonly int _bindingPower;
	private readonly bool _isRight;

	public BinaryOperatorParselet(int bindingPower, bool isRight)
	{
		_bindingPower = bindingPower;
		_isRight = isRight;
	}

	public Statement Parse(Parser parser, Statement left, Token token)
	{
		//right hand side slightly lower so that ^binary and 2^3 work correctly.
		Statement right = parser.ParseStatement(_bindingPower - (_isRight ? 1 : 0));
		// if (BinaryMathExpression.IsBinaryMathOperator(token.TokenType))
		// {
		// 	return new BinaryMathExpression(left, token.TokenType, right, token.Location);
		// }else if (BinaryBitwiseExpression.IsBinaryBitwiseOperator(token.TokenType))
		// {
		// 	return new BinaryBitwiseExpression(left, token.TokenType, right, token.Location);
		// }else if (BinaryConditionalExpression.IsBinaryConditionalOperator(token.TokenType))
		// {
		// 	return new BinaryConditionalExpression(left, token.TokenType, right,token.Location);
		// }
		var leftExp = left as Expression;
		var rightExp = right as Expression;
		//todo: errors on these
		return BinOp.GetBinaryOp(leftExp, token.Literal, rightExp);

		throw new ParseException($"Cannot parse {token.Literal} as binary operator at. ");
	}

	public int GetBindingPower()
	{
		return _bindingPower;
	}
}