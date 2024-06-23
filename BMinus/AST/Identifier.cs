
namespace BMinus.AST;

public class Identifier : Expression
{
	public readonly string Value;
	public Identifier(string id)
	{
		Value = id;
	}

	public override string ToString()
	{
		return Value;
	}
}