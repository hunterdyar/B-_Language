using System.Text;

namespace BMinus.AST.PrimitiveStatements;

public class CompoundStatement : Statement
{
	public readonly List<Statement> Statements;

	public CompoundStatement(List<Statement> statements)
	{
		Statements = statements;
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		sb.Append('{');
		foreach (var statement in Statements)
		{
			sb.Append(statement.ToString());
			sb.Append(';');
		}

		sb.Append('}');
		return sb.ToString();
	}
}