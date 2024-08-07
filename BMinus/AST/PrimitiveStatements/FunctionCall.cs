﻿using System.Text;

namespace BMinus.AST.PrimitiveStatements;

public class FunctionCall : Expression
{
	public readonly Identifier FunctionName;
	public Expression[] Arguments;//shit, is this 

	public FunctionCall(Identifier functionName, List<Expression> arguments)
	{
		FunctionName = functionName;
		Arguments = arguments.ToArray();
	}

	protected override string GetJSONName()
	{
		return "Call (" + FunctionName.Value + ")";
	}

	protected override IEnumerable<Statement> GetChildren()
	{
		return Arguments;
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append(FunctionName.ToString());
		sb.Append('(');
		for (int i = 0; i < Arguments.Length; i++)
		{
			sb.Append(Arguments[i]);
			if (i < Arguments.Length - 1)
			{
				sb.Append(',');
			}
		}

		sb.Append(')');
		return sb.ToString();
	}
}