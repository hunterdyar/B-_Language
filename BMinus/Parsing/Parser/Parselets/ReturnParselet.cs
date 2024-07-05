using BMinus.AST;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public class ReturnParselet : IPrefixParselet
{
	public Statement Parse(Parser parser, Token token)
	{
		//consumed the 
		if (parser.Peek(TokenType.EndStatement))
		{
			return new ReturnStatement(null);
		}
		
		parser.Consume(TokenType.LParen);
		Expression? value = null;
		if (!parser.Peek(TokenType.RParen))
		{
			var s = parser.ParseStatement();
			if (s is Expression e)
			{
				value = e;
			}
			else
			{
				throw new ParseException($"Can't return value {s}, return value needs to be expression.");
			}
		}
		parser.Consume(TokenType.RParen);
		return new ReturnStatement(value);
	}
}