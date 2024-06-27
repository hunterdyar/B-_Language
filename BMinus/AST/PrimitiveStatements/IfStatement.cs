using System.Text;

namespace BMinus.AST;

public class IfStatement : Statement
{
	public readonly Expression Condition;
	public Statement Consequence;

	public IfStatement(Expression condition, Statement consequence)
	{
		this.Condition = condition;
		Consequence = consequence;
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("if(");
		sb.Append(Condition.ToString());
		sb.Append(')');
		sb.Append(Consequence);
		return sb.ToString();
	}
}