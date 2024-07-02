using System.Text;

namespace BMinus.AST.PrimitiveStatements;

public class ProgramStatement : Statement
{
	public readonly Statement[] Statements;

	public ProgramStatement(Statement statement)
	{
		Statements = new[] { statement };
	}
	public ProgramStatement(Statement[] statements)
	{
		this.Statements = statements;
	}

	public ProgramStatement(List<Statement> statements)
	{
		this.Statements = statements.ToArray();
	}

	public override string GetJSON()
	{
		StringBuilder sb = new StringBuilder();
		sb.Append('{');
		sb.Append("\"name\":\"Program Statement\", ");
		sb.Append($"\"id\":\"{UID.ToString()}\", ");
		sb.Append("\"children\": [");
		for (var i = 0; i < Statements.Length; i++)
		{
			var statement = Statements[i];
			sb.Append(statement.GetJSON());
			if (i < Statements.Length - 1)
			{
				sb.Append(',');
			}
		}

		sb.Append("]}");
		return sb.ToString();
	}

	public override string ToString()
	{
		if (this.Statements.Length == 1)
		{
			return Statements[0].ToString();
		}
		
		StringBuilder stringBuilder = new StringBuilder();
		foreach (var s in Statements)
		{
			stringBuilder.Append(s.ToString());
			stringBuilder.Append(";");
		}

		return stringBuilder.ToString();
	}
}