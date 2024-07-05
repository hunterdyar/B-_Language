using System.Text;

namespace BMinus.AST;

public class ReturnStatement : Statement
{
	public Expression? Value;

	public ReturnStatement(Expression? value)
	{
		Value = value;
	}
	
	protected override IEnumerable<Statement> GetChildren()
	{
		if (Value != null)
		{
			return new[] { Value };
		}
		else
		{
			return Array.Empty<Statement>();
		}
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("return(");
		if (Value != null)
		{
			sb.Append(Value.ToString());
		}
		sb.Append(')');
		return sb.ToString();
	}
}