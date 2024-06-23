using System.Net.Mime;
using Superpower.Model;

namespace BMinus.AST.PrimitiveStatements;

public class Nop : Statement
{
	//Does Nothing
	public Nop(Position position) : base(position)
	{
	}
	public override string ToString()
	{
		return "(nop)";
	}
}