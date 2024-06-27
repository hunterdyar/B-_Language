namespace BMinus.AST;

public class Label : Statement
{
	public readonly string LabelID;

	public Label(string labelId)
	{
		LabelID = labelId;
	}

	public override string ToString()
	{
		return LabelID + ":\n";
	}
}