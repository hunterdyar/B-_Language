using System.Text;

namespace BMinus.AST.PrimitiveStatements;

public class ProgramStatement
{
	public readonly Statement[] Statements;

	public ProgramStatement(Statement[] statements)
	{
		this.Statements = statements;
	}

	public ProgramStatement(List<Statement> statements)
	{
		this.Statements = statements.ToArray();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (var s in Statements)
		{
			stringBuilder.Append(s.ToString());
			stringBuilder.Append(",");
		}

		return stringBuilder.ToString();
	}
}