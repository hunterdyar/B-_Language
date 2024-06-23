using BMinus.Parser;
using Superpower.Model;

namespace BMinus.AST;

public class WordLiteral : Expression
{
	private readonly byte[] Value;
	private readonly Position position;

	public WordLiteral(Token<BMToken> value, int intVal) : base(value.Position)
	{
		this.position = value.Position;
		string valLit = value.ToString();
		Value = BitConverter.GetBytes(intVal);
	}

	public override string ToString()
	{
		return BitConverter.ToInt32(Value).ToString();
	}
}