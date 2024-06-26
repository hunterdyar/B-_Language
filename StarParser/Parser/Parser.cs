using Ara3D.Utils;
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
			var e = b.Consume(TokenType.EndStatement);
			lex.From(b);
			statement = assignment;
			return e;
		}
		
		var a = lex.Clone();
		if (TryIdentifier(a, out var id))
		{
			var end = a.Consume(TokenType.EndStatement);
			statement = id;
			lex.From(a);
			return end;
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
		if (ParserUtility.OneOrMore(TryCreateStatement, lex, out var statements))
		{
			statement = new ProgramStatement(statements);
			return true;
		}

		statement = null;
		return false;
	}
}

