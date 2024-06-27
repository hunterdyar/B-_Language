using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public class StatementBlockParselet : IPrefixParselet
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
				//todo: this is matched with parseProgram. move to "parseexpressionsequence" generic fn.
				if (s is not StatementBlock)
				{
					parser.Consume(TokenType.EndStatement);
				}
				statements.Add(s);
				
			} while (!parser.Match(TokenType.RBrace));//match consume on match
		}
		
		return new StatementBlock(statements);
	}
}