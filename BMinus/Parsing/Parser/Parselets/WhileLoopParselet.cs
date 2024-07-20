using BMinus.AST;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public class WhileLoopParselet : IPrefixParselet
{
	public Statement Parse(Parser parser, Token token)
	{
		parser.Consume(TokenType.LParen);
		var cond = parser.ParseStatement();
		if (!(cond is Expression condition))
		{
			throw new ParseException($"Unable to Parse while loop. is {cond} an expression?");
		}

		parser.Consume(TokenType.RParen);

		var consequence = parser.ParseStatement();
		return new WhileLoop(condition, consequence);
	}
}