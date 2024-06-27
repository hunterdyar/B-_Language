using System.Diagnostics.CodeAnalysis;
using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using StarParser.Tokenizer;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Identifier = BMinus.AST.Identifier;

using ST = StarParser.Tokenizer.TokenType;
using Token = Superpower.Parsers.Token;

namespace StarParser.Parser;

public static class Parser
{
	public static bool TryParse(string input, out Statement value, [MaybeNullWhen(true)] out string error,
		out Position errorPosition)
	{
		var tokens = SupLexer.Instance.TryTokenize(input);
		if (!tokens.HasValue)
		{
			value = null;
			error = tokens.ToString();
			errorPosition = tokens.ErrorPosition;
			return false;
		}

		var p = ProgramParser.TryParse(tokens.Value);
		if (!p.HasValue)
		{
			value = null;
			error = p.ToString();
			errorPosition = p.ErrorPosition;
			return false;
		}
		
		value = p.Value;
		error = null;
		errorPosition = p.ErrorPosition;
		return true;
	}
	
	public static TokenListParser<ST, Statement> StatementParser =>
		from i in IdentifierParser.Or(IntLitParser)
		from semi in Token.EqualTo(TokenType.EndStatement)
		select (Statement)i;

	public static TokenListParser<ST, ProgramStatement> ProgramParser =>
		from x in StatementParser.Many()
		select new ProgramStatement(x);
	
	
	//Expressions
	public static TokenListParser<ST, Expression> IdentifierParser =>
		from t in Token.EqualTo(TokenType.Identifier)
		select (Expression)new Identifier(t.ToStringValue());

	public static TokenListParser<ST, Expression> IntLitParser =>
		from t in Token.EqualTo(TokenType.IntLiteral)
		select (Expression)new WordLiteral('i',t.ToStringValue());
	
}