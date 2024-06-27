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

	public WordLiteral(char t, string s)
	{
		if (t == 'i')
		{
			if (int.TryParse(s, out var v))
			{
				Value = BitConverter.GetBytes(v);
			}
		}
		else
		{
			throw new ArgumentException($"Unable to parse {s} as an integer");
		}
	}


	public override string ToString()
	{
		return BitConverter.ToInt32(Value).ToString();
	}
}