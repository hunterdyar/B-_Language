using BMinus.AST;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public class ParenthesizedExpressionParselet : IPrefixParselet
{
	public Statement Parse(Parser parser, Token token)
	{
		//consume (
		var state = parser.ParseStatement();
		if (state is Expression expr)
		{
			parser.Consume(TokenType.RParen);
			return expr;
		}

		throw new ParseException("Unable to parse parenthesized expression (is statement?)");
	}
}