using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Identifier = Superpower.Parsers.Identifier;

namespace BMinus.Parser;

public class StatementParser
{

	public static TokenListParser<BMToken, Statement> AssignmentParser =
			from id in ExpressionParsers.IdentifierParser
			from eq in Token.EqualTo(BMToken.Assign)
			from exp in ExpressionParsers.ExpressionParser
			select (Statement)new Assignment(id, exp, eq.Position);

	public static TokenListParser<BMToken, Statement> Statement =
		from s in AssignmentParser
		select s;
}