using Superpower.Model;

namespace BMinus.AST;

public abstract class BinOp : Expression
{
	public readonly Expression Left;
	public readonly Expression Right;
	public readonly Position Position;
	public virtual string OpAsString => "UDEFINED";
	protected BinOp(Expression left, Expression right, Position position) : base(position)
	{
		Left = left;
		Right = right;
	}
	public override string ToString()
	{
		return $"({Left.ToString()} {OpAsString} {Right.ToString()} )";
	}
}