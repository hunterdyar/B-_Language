using System;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using BMinus.VirtualMachine;
using Env = BMinus.Environment.Environment;
Console.WriteLine("Initialized");

public partial class BMinusRuntime
{
	private static VMRunner _runner;

	[JSExport]
	public static void Init()
	{
		_runner = new VMRunner();
		_runner.OnOutputChange += SendOutput;
	}
	
	
	[JSExport]
	public static void RunProgram(string program)
	{
		var output = _runner.RunProgram(program);
		SendOutput(output);
	}

	[JSExport]

	public static void CompileAndStep(string program)
	{
		Console.WriteLine("Compiling");
		_runner.Compile(program);
		_runner.Step();
	}

	[JSExport]
	public static void Step()
	{
		_runner.Step();
	}
	
	[JSExport]
	public static int[] GetGlobals()
	{
		return _runner.Env.Globals.Values.ToArray();
	}

	[JSExport]
	public static int GetState()
	{
		return (int)_runner.VMState;
	}
	
	[JSImport("onOutput", "main.js")]
	public static partial void SendOutput(string newLine);


	[JSImport("window.location.href", "main.js")]
	internal static partial string GetHRef();
}