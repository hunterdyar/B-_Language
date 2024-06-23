using Superpower.Model;

namespace BMinus.AST;

public class AddExpr : BinOp
{
	public override string OpAsString => "+";

	public AddExpr(Expression left, Expression right, Position position) : base(left, right, position)
	{
	}
	
}