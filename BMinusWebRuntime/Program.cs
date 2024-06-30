using System;
using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using BMinus.Compiler;
using BMinus.Parser;
using BMinus.Tokenizer;
using BMinus.VirtualMachine;

Console.WriteLine("Hello, Browser!");

public partial class MyClass
{
	
	[JSExport]
	public static string RunProgram(string program)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		try
		{
			Parser p = new Parser(new Lexer(program));
			var tree = p.Parse();
			Compiler c = new Compiler(tree);
			stopwatch.Stop();
			var env = c.GetEnvironment();
			VirtualMachine vm = new VirtualMachine(env, true);
			vm.VMConsole.AppendLine($"Compiled in {stopwatch.ElapsedMilliseconds}ms");
			vm.Run();
			return vm.VMConsole.ToString();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return e.Message;
			throw;
		}
	}

	[JSImport("window.location.href", "main.js")]
	internal static partial string GetHRef();
}