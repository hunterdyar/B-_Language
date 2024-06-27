﻿namespace BMinus.AST.PrimitiveStatements;

public class Assignment : Statement
{
	public Identifier Identifier;
	public Expression ValueExpr;

	public Assignment(Statement id, Expression valueExpr)
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
}