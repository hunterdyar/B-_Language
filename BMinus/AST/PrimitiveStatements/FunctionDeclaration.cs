using System.Data.Common;
using System.Text;

namespace BMinus.AST.PrimitiveStatements;

public class FunctionDeclaration : Statement
{
	public readonly Identifier Identifier;
	public readonly Identifier[] Parameters;
	public readonly int ArgCount;
	public readonly Statement Statement;

	public FunctionDeclaration(Identifier id, List<Identifier> arguments, Statement statement)
	{
		Identifier = id;
		this.Parameters = arguments.ToArray();
		ArgCount = this.Parameters.Length;
		this.Statement = statement;
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append(Identifier.ToString());
		sb.Append('(');
		for (int i = 0; i < ArgCount; i++)
		{
			sb.Append(Parameters[i]);
			if (i < ArgCount - 1)
			{
				sb.Append(',');
			}
		}

		sb.Append(')');
		sb.Append(Statement.ToString());
		return sb.ToString();
	}
}