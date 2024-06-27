using BMinus.AST;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public class LiteralParselet : IPrefixParselet
{
	public Statement Parse(Parser parser, Token token)
	{
		if (token.TokenType == TokenType.IntLiteral)
		{
			return new WordLiteral('i',token.Literal);
		}

		if (token.TokenType == TokenType.String)
		{
			return new WordLiteral('s',token.Literal);
		}

		if (token.TokenType == TokenType.HexLiteral)
		{
			return new WordLiteral('h',token.Literal);
		}
		
		throw new ParseException("uh oh!");
	}
}