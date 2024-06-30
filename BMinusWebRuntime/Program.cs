using System;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using BMinus.VirtualMachine;
using Env = BMinus.Environment.Environment;
Console.WriteLine("Initialized");

public partial class BMinusRuntime
{
	private static VMRunner _runner = new VMRunner();
	public BMinusRuntime()
	{
		//onOutput +=
	}
	
	[JSExport]
	public static void RunProgram(string program)
	{
		var output = _runner.RunProgram(program);
		OnOutput(output);
	}

	[JSImport("onOutput", "main.js")]
	public static partial void OnOutput(string newLine);

	[JSExport]
	public static int[] GetGlobals()
	{
		return _runner.Env.Globals.Values.ToArray();
	}

	[JSImport("window.location.href", "main.js")]
	internal static partial string GetHRef();
}