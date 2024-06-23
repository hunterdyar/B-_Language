using System.Globalization;

namespace BMinus.AST;

public class WordLiteral : Expression
{
	private readonly byte[] Value;

	public WordLiteral( int intVal)
	{
		Value = BitConverter.GetBytes(intVal);
	}
	

	public WordLiteral(double dVal)
	{
		Value = BitConverter.GetBytes(dVal);
	}
	

	public override string ToString()
	{
		return BitConverter.ToInt32(Value).ToString();
	}
}