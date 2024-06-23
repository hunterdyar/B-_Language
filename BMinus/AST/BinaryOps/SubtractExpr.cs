using Superpower.Model;

namespace BMinus.AST;

public class SubtractExpr : BinOp
{
	public override string OpAsString => "-";

	public SubtractExpr(Expression left, Expression right, Position position) : base(left, right, position)
	{
	}
}