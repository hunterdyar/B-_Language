namespace BMinus.AST;

public class IfElseStatement : IfStatement
{
	public readonly Statement Alternative;

	public IfElseStatement(Expression condition, Statement consequence, Statement alternative) : base(condition, consequence)
	{
		this.Alternative = alternative;
	}

	public override string ToString()
	{
		var s = base.ToString();
		return s + " else " + Alternative.ToString();
	}
}