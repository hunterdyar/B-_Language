using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using StarParser.Tokenizer;

namespace StarParser.Parser;

public static partial class Parser
{
	public static Statement Parse(Lexer lexer)
	{
		var wrapper = new LexerWrapper(lexer);
		if (TryProgram(new LexerState(wrapper), out var program))
		{
			return program;
		}

		return null;
	}

	public static bool TryCreateStatement(LexerState lex, out Statement statement)
	{
		var b = lex.Clone();
		if (TryAssignment(b, out var assignment))
		{
			lex.From(b);
			statement = assignment;
			return true;
		}
		
		var a = lex.Clone();
		if (TryIdentifier(a, out var id))
		{
			statement = id;
			lex.From(a);
			return true;
		}
		statement = null;
		return false;
	}

	public static bool TryIdentifier(LexerState lex, out Identifier id)
	{
		var t = lex.NextToken();
		if (t.TokenType == TokenType.Identifier)
		{
			id = new Identifier(t.Literal);
			return true;
		}

		id = null;
		return false;
	}

	
	
	

	public static bool TryAssignment(LexerState lex, out Assignment s)
	{
		if(TryIdentifier(lex, out var id))
		{
			if (!lex.Consume(TokenType.Assignment))
			{
				s = null;
				return false;
			}

			if (TryExpression(lex, out var e))
			{
				s = new Assignment((Identifier)id, e);
				return true;
			}
			
		}

		s = null;
		return false;
	}

	public static bool TryProgram(LexerState lex, out Statement statement)
	{
		List<Statement> statements = new List<Statement>();
		while (TryCreateStatement(lex, out var s))
		{
			statements.Add(s);
		}
		statement = new ProgramStatement(statements);
		return statements.Any();
	}
}

