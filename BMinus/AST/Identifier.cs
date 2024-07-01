
using Microsoft.VisualBasic;

namespace BMinus.AST;

public class Identifier : Expression
{
	public readonly string Value;
	public Identifier(string id) : base()
	{
		Value = Strings.Trim(id);
	}

	public override string ToString()
	{
		return Value;
	}
}