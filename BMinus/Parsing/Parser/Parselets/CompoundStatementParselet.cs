using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public class CompoundStatementParselet : IPrefixParselet
{
	public Statement Parse(Parser parser, Token token)
	{
		List<Statement> statements = new List<Statement>();
		if(!parser.Peek(TokenType.RBrace))
		{
			do
			{
				//eat a statement inside the braces{a;b;}
				var s = parser.ParseStatement();
				if (Parser.StatementRequiresSemicolon(s))
				{
					parser.Consume(TokenType.EndStatement);
				}
				statements.Add(s);
				
			} while (!parser.Match(TokenType.RBrace));//match consume on match
		}
		else
		{
			//empty {} is valid
			parser.Match(TokenType.RBrace);
		}
		
		return new CompoundStatement(statements);
	}
}