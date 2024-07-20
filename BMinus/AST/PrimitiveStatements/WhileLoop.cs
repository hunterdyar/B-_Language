using System.Text;

namespace BMinus.AST;

public class WhileLoop : Statement
{
	public readonly Expression Condition;
	public Statement Consequence;

	public WhileLoop(Expression condition, Statement consequence)
	{
		this.Condition = condition;
		this.Consequence = consequence;
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("while(");
		sb.Append(Condition.ToString());
		sb.Append(')');
		sb.Append(Consequence);
		return sb.ToString();
	}
}
