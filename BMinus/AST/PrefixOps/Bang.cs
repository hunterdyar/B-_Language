namespace BMinus.AST;

public class Bang : PrefixOp
{
	public Bang(Expression right) : base(right)
	{
	}

	public override string ToString()
	{
		return "!(" + Right.ToString() + ")";
	}
}