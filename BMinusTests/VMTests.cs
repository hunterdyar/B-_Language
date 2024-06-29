using System.Diagnostics;
using BMinus.Compiler;
using BMinus.Parser;
using BMinus.Tokenizer;
using BMinus.VirtualMachine;

namespace BMinusTests;

public class VMTests
{
	public class ConsoleOutput : IDisposable
	{
		private StringWriter stringWriter;
		private TextWriter originalOutput;

		public ConsoleOutput()
		{
			stringWriter = new StringWriter();
			originalOutput = Console.Out;
			Console.SetOut(stringWriter);
		}

		public string GetOuput()
		{
			return stringWriter.ToString();
		}

		public void Dispose()
		{
			Console.SetOut(originalOutput);
			stringWriter.Dispose();
		}
	}
	public static void RunTestOnOutput(string program, string expectedOutput)
	{
		using(var output = new ConsoleOutput()){
			//run test
			Parser p = new Parser(new Lexer(program));
			var tree = p.Parse();
			Compiler c = new Compiler(tree);
			var env = c.GetEnvironment();
			VirtualMachine vm = new VirtualMachine(env);
			vm.Run();

			Assert.That(output.GetOuput(), Is.EqualTo(expectedOutput));
		}
	}

	[Test]
	[TestCase("a=4;","4")]
	[TestCase("a=2+2;", "4")]
	[TestCase("a=2+2*2;", "6")]
	[TestCase("a=2;a=3;a=4;a=6;", "6")]

	public static void TestVarA(string p, string e)
	{
		p = "auto a;" + p + "putchar(a);";
		RunTestOnOutput(p,e);
	}

	[Test]
	[TestCase("""
		main(){
			putchar(1);
		};
		main();
	""", "1")]

	public static void FuncDecTest(string p, string e)
	{
		RunTestOnOutput(p, e);
	}
	
	//test that it fails on unclosed a "
}