using System.Net.Mime;
using Superpower.Model;

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