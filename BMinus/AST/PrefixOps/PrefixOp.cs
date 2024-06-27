namespace BMinus.AST;

public class PrefixOp : Expression
{
	public readonly Expression Right;

	public PrefixOp(Expression right)
	{
		this.Right = right;
	}
	public static PrefixOp GetPrefixOp(Expression right, string op)
	{
		switch (op)
		{
			case "-":
				//check if right is literal (optimize!)
				return new Negate(right);
			case "!":
				return new Bang(right);
		}
		
		throw new ArgumentException($"invalid prefix '{op}'");
	}
}