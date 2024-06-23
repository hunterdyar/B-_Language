namespace BMinus.AST.PrimitiveStatements;

public class Nop : Statement
{
	//Does Nothing
	public Nop( )
	{
	}
	public override string ToString()
	{
		return "(nop)";
	}
}