using System.Text;

namespace BMinus.AST.PrimitiveStatements;

public class ExternDeclaration : Statement
{
	public Identifier[] Identifiers => _identifiers;
	private Identifier[] _identifiers;

	public ExternDeclaration(Identifier[] ids)
	{
		if (ids == null || ids.Length == 0)
		{
			throw new ArgumentException($"Null or no id's to declare?");
		}

		_identifiers = ids;
	}

	public ExternDeclaration(List<Identifier> ids)
	{
		if (ids == null || ids.Count == 0)
		{
			throw new ArgumentException($"Null or no id's to declare?");
		}

		_identifiers = ids.ToArray();
	}

	public ExternDeclaration(Identifier id)
	{
		_identifiers = new[] { id };
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("extern ");
		for (var i = 0; i < _identifiers.Length; i++)
		{
			var id = _identifiers[i];
			sb.Append(id.Value);
			if (i < _identifiers.Length - 1)
			{
				sb.Append(",");
			}
		}

		return sb.ToString();
	}
}