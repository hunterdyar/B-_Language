using BMinus.Models;

namespace BMinus.AST;

public class PrefixOp : Expression
{
	public readonly Expression Right;
	public readonly UnaryPrefixOp Op;
	public PrefixOp(Expression right, UnaryPrefixOp op)
	{
		this.Right = right;
		Op = op;
	}
	public static PrefixOp GetPrefixOp(Expression right, string op)
	{
		switch (op)
		{
			case "-":
				//check if right is literal (optimize!)
				return new PrefixOp(right, UnaryPrefixOp.Negate);
			case "!":
				return new PrefixOp(right, UnaryPrefixOp.Not);
		}
		
		throw new ArgumentException($"Unable to create prefix expression with op: '{op}'");
	}
}