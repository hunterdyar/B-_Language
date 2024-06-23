﻿using System.Text;
using Superpower.Model;

namespace BMinus.AST.PrimitiveStatements;

public class ProgramStatement : Statement
{
	public readonly Statement[] Statements;

	public ProgramStatement(Statement statement) : base(statement.Position)
	{
		Statements = new[] { statement };
	}
	public ProgramStatement(Statement[] statements, Position position) : base(position)
	{
		this.Statements = statements;
	}

	public ProgramStatement(List<Statement> statements, Position position) : base(position)
	{
		this.Statements = statements.ToArray();
	}

	public override string ToString()
	{
		if (this.Statements.Length == 1)
		{
			return Statements[0].ToString();
		}
		
		StringBuilder stringBuilder = new StringBuilder();
		foreach (var s in Statements)
		{
			stringBuilder.Append(s.ToString());
			stringBuilder.Append(";");
		}

		return stringBuilder.ToString();
	}
}