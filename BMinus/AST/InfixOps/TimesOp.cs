namespace BMinus.AST;

public class TimesOp : BinOp
{
	public override string OpAsString => "*";

	public TimesOp(Expression left, Expression right) : base(left, right)
	{
	}
}