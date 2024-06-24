namespace BMinus.AST;

public class Negate : PrefixOp
{
	public Negate(Expression right) : base(right)
	{
	}

	public override string ToString()
	{
		return "-" + Right.ToString() + "";
	}
}