using System.Text;

namespace BMinus.AST.PrimitiveStatements;

public class VariableDeclaration : Statement
{
	public Identifier[] Identifiers => _identifiers;
	private Identifier[] _identifiers;
	
	public VariableDeclaration(Expression[] ids)
	{
		if (ids == null || ids.Length == 0)
		{
			throw new ArgumentException($"Null or no id's to declare");
		}

		_identifiers = ids.Select(e=>e as Identifier).ToArray();
	}

	public VariableDeclaration(Expression idExpr)
	{
		if (idExpr is Identifier id)
		{
			_identifiers = new[] { id };
		}
		else
		{
			throw new ArgumentException($"{idExpr} is not an identifier.");
		}
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("var ");
		for (var i = 0; i < _identifiers.Length; i++)
		{
			var id = _identifiers[i];
			sb.Append(id.Value);
			if (i < _identifiers.Length-1)
			{
				sb.Append(",");
			}
		}

		return sb.ToString();
	}
}