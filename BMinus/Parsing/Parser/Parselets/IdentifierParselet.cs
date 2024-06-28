using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public class IdentifierParselet : IPrefixParselet
{
	public Statement Parse(Parser parser, Token token)
	{
		if (parser.Peek(TokenType.RBrace))
		{
			var indexer = parser.ParseStatement();
			if (!(indexer is Expression index))
			{
				throw new ParseException("Vectors must be declared with valid expression for their size");
			}

			parser.Consume(TokenType.RBrace);
			return new VectorIdentifier(token.Literal, index);
		}
		//else if: parser.peek IsLiteral
		//constant or vector declaration -
		//a 'hi'; is valid
		//v[10] 'hi!', 1, 2, 3, 0777; is valid (0 is hi, numbers in 1-4, 5-10 are not initialized.
		//todo: constants
		return new Identifier(token.Literal);
	}
}