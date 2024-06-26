using BMinus.AST;
using StarParser.Tokenizer;

namespace StarParser.Parser;

public class ParserUtility
{
	public delegate bool TryParserFunc(LexerState lex, out Statement statement);

	public static bool ZeroOrMore(TryParserFunc func,LexerState lex, out List<Statement> statements)
	{
		statements = new List<Statement>();
		while (func(lex, out var s))
		{
			statements.Add(s);
		}

		return true;
	}

	public static bool OneOrMore(TryParserFunc func, LexerState lex, out List<Statement> statements)
	{
		statements = new List<Statement>();
		while (func(lex, out var s))
		{
			statements.Add(s);
		}

		return statements.Count>=1;
	}
}