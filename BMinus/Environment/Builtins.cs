namespace BMinus.Environment;
using VM = VirtualMachine.VirtualMachine;
public static class Builtins
{
	public delegate int Builtin(VM vm, params int[] args);

	public static bool IsBuiltin(string name, out int index)
	{
		index = _builtins.FindIndex(x => x.Item1 == name);
		return index >= 0;//-1 when can't find.
	}

	public static void CallBuiltin(VM vm, int builtIndex, params int[] args)
	{
		_builtins[builtIndex].Item2.Invoke(vm,args);
	}
	
	private static readonly List<(string, Builtin)> _builtins = new List<(string, Builtin)>()
	{
		("putchar",Putchar),
		("putint",PutInt)
	};

	public static string GetBuiltinName(int id)
	{
		return _builtins[id].Item1;
	}
	public static int Putchar(VM vm, params int[] args)
	{
		foreach (int i in args)
		{
			vm.Runner.Append(System.Text.Encoding.UTF8.GetString(BitConverter.GetBytes(i)));
		}
		return 1;
	}

	public static int PutInt(VM vm, params int[] args)
	{
		foreach (int i in args)
		{
			vm.Runner.Append(i.ToString());
		}

		return 1;
	}

	
}