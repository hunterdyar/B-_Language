namespace BMinus.Environment;

public static class Builtins
{
	public delegate int Builtin(params int[] args);

	public static bool IsBuiltin(string name, out int index)
	{
		index = _builtins.FindIndex(x => x.Item1 == name);
		return index >= 0;//-1 when can't find.
	}

	public static void CallBuiltin(int builtIndex, params int[] args)
	{
		_builtins[builtIndex].Item2.Invoke(args);
	}
	
	private static readonly List<(string, Builtin)> _builtins = new List<(string, Builtin)>()
	{
		("putchar",Putchar)
	};

	public static int Putchar(params int[] args)
	{
		foreach (int i in args)
		{
			Console.Write(i);
		}

		return 0;
	}
}