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
			Console.Write(runner.VMConsole.ToString());
			Assert.That(result, Is.EqualTo(expectedOutput));
	}

	[Test]
	[TestCase("a=4;","4")]
	[TestCase("a=2+2;", "4")]
	[TestCase("a=2+2*2;", "6")]
	[TestCase("a=2;b=3;c=4;a=a+c;", "6")]
	public static void IntTestVarA(string p, string e)
	{
		p = "auto a,b,c;" + p + "putint(a);";
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
	[TestCase("""
	          double(v) {
	          	return (v * 2);
	          }

	          auto a;
	          a = 1;

	          a = double(4);
	          putint(a);
	          ""","8")]

	[TestCase("""
	          double(a){
	            return(a*2);
	          }
	          main(a,b,c){
	            putint(double(a)+double(b)+double(c));
	          }
	          main(1,2,3);
	          """, "12")]

	[TestCase("""
	          double(a){
	            return(a*2);
	          }
	          add(a,b){
	            return(a+b);
	          }
	          main(a,b,c){{
	            putint(add(add(b,a),add(c,c)));
	          }}
	          main(1,2,3);
	          """, "9")]

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
	[Test]
	[TestCase("""
	          foo(a,b){
	            auto c;
	            c = a+b;
	            auto d;
	            d = c+c;
	            return(d);
	          }
	          putint(foo(1,3));
	          """, "8")]

	public static void FuncLocals(string p, string e)
	{
		RunTestOnOutput(p, e);
	}

	[Test]
	[TestCase("""
	          auto a;
	          a = 1;
	          print(a);
	          
	          print(c){
	            putint(c);
	          }
	          """, "1")]

	public static void UnknownFuncTest(string p, string e)
	{
		RunTestOnOutput(p, e);
	}

	[Test]
	[TestCase("""
	          print(){
	            extern a;
	            a = 5;
	            putint(a);
	          }
	          
	          auto a;
	          a = 1;
	          print();
	          """, "5")]
	[TestCase("""
	          changea(){
	            extern a;
	            a = 5;
	          }

	          auto a;
	          a = 1;
	          changea();
	          putint(a);
	          """, "5")]

	[TestCase("""
	          auto a;
	          a = 2;
	          a = double(1) + double(a);
	          
	          double(f){
	            auto c,d;
	            c = f;
	            d = f;
	            return(c+d);
	          }
	          putint(a);
	          """, "6")]

	public static void UnknownExternTest(string p, string e)
	{
		RunTestOnOutput(p, e);
	}

	[TestCase("""
	          auto a;
	          a = 5;

	          while(a > 0){
	            putint(a);
	            a = a-1;
	          }
	          putint(0);
	          """, "543210")]

	[TestCase("""
	          auto a;
	          a = 5;

	          while(a != 0){
	            putint(a);
	            a = a-1;
	          }
	          putint(0);
	          """, "543210")]

	public static void WhileLoopTest(string p, string e)
	{
		RunTestOnOutput(p, e);
	}

	
}