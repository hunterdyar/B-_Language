using Superpower.Model;

namespace BMinus.AST;

public class Statement
{
	public Position Position;

	public Statement(Position position)
	{
		Position = position;
	}

	public override string ToString()
	{
		return "";
	}
}