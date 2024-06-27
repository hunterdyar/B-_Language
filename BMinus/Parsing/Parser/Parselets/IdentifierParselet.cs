using BMinus.AST;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public class IdentifierParselet : IPrefixParselet
{
	public Statement Parse(Parser parser, Token token)
	{
		// switch (token.TokenType)
		// {
		// 	case TokenType.TrueKeyword:
		// 		return new BoolLiteralExpression(token.TokenType, token.Location);
		// 	case TokenType.FalseKeyword:
		// 		return new BoolLiteralExpression(token.TokenType, token.Location);
		// 	case TokenType.NullKeyword:
		// 		return new NullExpression(token.Location);
		// 	//null
		// }
		return new Identifier(token.Literal);
	}
}