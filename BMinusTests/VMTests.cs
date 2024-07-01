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
			//run test
			VMRunner runner = new VMRunner();
			var result = runner.RunProgram(program);
			Assert.That(result, Is.EqualTo(expectedOutput));
	}

	[Test]
	[TestCase("a=4;","4")]
	[TestCase("a=2+2;", "4")]
	[TestCase("a=2+2*2;", "6")]
	[TestCase("a=2;a=3;a=4;a=6;", "6")]
	public static void IntTestVarA(string p, string e)
	{
		p = "auto a;" + p + "putint(a);";
		RunTestOnOutput(p,e);
	}

	[Test]
	[TestCase("if(0){putint(1);}", "")]
	[TestCase("if(1){putint(1);}", "1")]
	[TestCase("if(0){putint(1);}else{putint(2);}", "2")]
	[TestCase("if(1){putint(1);}else{putint(2);}", "1")]

	public static void IfTest(string p, string e)
	{
		RunTestOnOutput(p, e);
	}

	[Test]
	[TestCase("""
		main(){
			putint(1);
		};
		main();
	""", "1")]

	public static void FuncDecTest(string p, string e)
	{
		RunTestOnOutput(p, e);
	}
	
	[Test]
	[TestCase("""
	          	main(){
	          		putint(1);
	          	};
	          	main();
	          """)]
	public static void StepTest(string p)
	{
		VMRunner runner = new VMRunner();
		runner.Compile(p);
		runner.Step();
		runner.Step();
		runner.Step();
		runner.Step();
		runner.Step();
		runner.Step();

	}
	
	//test that it fails on unclosed a "
}