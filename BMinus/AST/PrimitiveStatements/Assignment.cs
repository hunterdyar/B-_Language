using System.Text;

namespace BMinus.AST.PrimitiveStatements;

public class Assignment : Statement
{
	public Identifier Identifier;
	public Expression ValueExpr;

	public Assignment(Statement id, Expression valueExpr) : base()
	{
		if (id is Identifier idexp)
		{
			Identifier = idexp;
		}
		else
		{
			throw new ArgumentException($"{id} is not an identifier. Cannot assign to it.");
		}

		ValueExpr = valueExpr;
	}

	public override string ToString()
	{
		return $"{Identifier.Value} = {ValueExpr.ToString()}";
	}

	protected override IEnumerable<Statement> GetChildren()
	{
		return new[] { ValueExpr };
	}
}