using System.Globalization;

namespace BMinus.AST;

public class WordLiteral : Expression
{
	public readonly byte[] Value;
	public int ValueAsInt => BitConverter.ToInt32(Value);

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
		}else if (t == 's')
		{
			throw new NotImplementedException("String Literals not implemented yet");
		}
		else if (t == 'h')
		{
			throw new NotImplementedException("Hex Literals not implemented yet");
		}else if (t == 'c')
		{
			//'hi'
			throw new NotImplementedException("Char Literals not implemented yet");
		}
		else
		{
			throw new ArgumentException($"Unable to parse {s} as a literal.");
		}
	}



	public override string ToString()
	{
		return BitConverter.ToInt32(Value).ToString();
	}
}