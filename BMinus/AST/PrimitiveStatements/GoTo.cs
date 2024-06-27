namespace BMinus.AST;

public class GoTo : Statement
{
	public Identifier GotoLabel;

	public GoTo(Identifier gotoLabel)
	{
		GotoLabel = gotoLabel;
	}
	//runtime: get and cache label.
	
}