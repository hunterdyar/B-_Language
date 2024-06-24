
namespace BMinus.AST;

public class AddExpr : BinOp
{
	public override string OpAsString => "+";

	public AddExpr(Expression left, Expression right) : base(left,right)
	{
	}
	
}