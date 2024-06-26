
namespace BMinus.AST;

public abstract class BinOp : Expression
{
	public readonly Expression Left;
	public readonly Expression Right;
	public virtual string OpAsString => "UDEFINED";
	protected BinOp(Expression left, Expression right)
	{
		Left = left;
		Right = right;
	}
	public override string ToString()
	{
		return $"({Left.ToString()} {OpAsString} {Right.ToString()})";
	}

	public static BinOp GetBinaryOp(Expression left, string op, Expression right)
	{
		switch (op)
		{
			case "+":
				return new AddExpr(left, right);
			case "-":
				return new SubtractExpr(left, right);
			case "*":
				return new TimesOp(left, right);
			default:
				throw new ArgumentException($"bad token: {op}");
		}
	}
	
}