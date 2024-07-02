namespace BMinus.AST;

public class TernaryOp : Expression
{
	public readonly Expression Condition;
	public readonly Expression Consequence;
	public readonly Expression Alternative;

	public TernaryOp(Expression condition, Expression consequence, Expression alternative)
	{
		Condition = condition;
		Consequence = consequence;
		Alternative = alternative;
	}

	protected override IEnumerable<Statement> GetChildren()
	{
		return new[] { Condition, Consequence, Alternative };
	}

	public override string ToString()
	{
		return Condition.ToString() + " ? " + Consequence.ToString() + " : " + Alternative.ToString();
	}
}