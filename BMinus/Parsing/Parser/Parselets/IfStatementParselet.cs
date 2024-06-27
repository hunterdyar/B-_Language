using BMinus.AST;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public class IfStatementParselet : IPrefixParselet
{
	public Statement Parse(Parser parser, Token token)
	{
		parser.Consume(TokenType.LParen);
		var conds = parser.ParseStatement();
		if (!(conds is Expression condition))
		{
			throw new ParseException($"Unable to parse if statement. is {conds} an expression?");
		}

		parser.Consume(TokenType.RParen);

		var consequence = parser.ParseStatement();
		if (parser.Peek(TokenType.ElseKeyword))
		{
			parser.Consume();
			var alternative = parser.ParseStatement();
			return new IfElseStatement(condition, consequence, alternative);
		}

		return new IfStatement(condition, consequence);
	}
}