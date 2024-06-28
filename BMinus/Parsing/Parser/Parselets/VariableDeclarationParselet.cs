using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public class VariableDeclarationParselet : IPrefixParselet
{
	public Statement Parse(Parser parser, Token token)
	{
		var variables = new List<Identifier>();
		if (!parser.Peek(TokenType.EndStatement))
		{
			do
			{
				var idTok = parser.Consume(TokenType.Identifier);
				if (parser.Peek(TokenType.LBrace))
				{
					parser.Consume(TokenType.LBrace);
					var vSizeS = parser.ParseStatement();
					if (!(vSizeS is Expression vSize))
					{
						throw new ParseException("Vectors must be declared with valid expression for their size");
					}

					parser.Consume(TokenType.RBrace);
					variables.Add(new VectorIdentifier(idTok.Literal,vSize));
				}
				variables.Add(new Identifier(idTok.Literal));
			} while (parser.Match(TokenType.Comma));
		}

		if (variables.Count == 0)
		{
			throw new ParseException(
				$"Empty variable declaration. Must declare at least one variable after {token.Literal}");
		}
		else
		{
			return new VariableDeclaration(variables);
		}
	}
}