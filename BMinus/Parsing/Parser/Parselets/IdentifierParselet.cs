using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public class IdentifierParselet : IPrefixParselet
{
	public Statement Parse(Parser parser, Token token)
	{
		
		if (parser.Peek(TokenType.Colon))
		{
			parser.Consume();
			return new Label(token.Literal);
		}
		//todo: constants
		return new Identifier(token.Literal);
	}
}