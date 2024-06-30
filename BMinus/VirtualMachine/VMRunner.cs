using System.Diagnostics;
using System.Text;
using BMinus.Tokenizer;

namespace BMinus.VirtualMachine;

//a VM Runner is a wrapper to the VM. Let's you grab the output, event subscriptions, and so on.
public class VMRunner
{
	private Parser.Parser _parser;
	private Compiler.Compiler _compiler = new Compiler.Compiler();
	private VirtualMachine _vm;
	public Environment.Environment Env => _env;
	private Environment.Environment _env;

	private Stopwatch _compileWatch = new Stopwatch();

	public StringBuilder VMConsole => _vmConsole;
	private StringBuilder _vmConsole = new StringBuilder();
	
	//todo: move measuring and reporting to the runner.
	public string RunProgram(string program, bool report = false)
	{
		try
		{
			_compileWatch.Start();
			_parser = new Parser.Parser(new Lexer(program));
			var tree = _parser.Parse();
			_compiler.NewCompile(tree);
			_compileWatch.Stop();
			var env = _compiler.GetEnvironment();
			VirtualMachine vm = new VirtualMachine(env, this,report);
			_env = vm.Env;
			vm.Run();
			return VMConsole.ToString();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return e.Message;
		}
	}

	public void Append(string s)
	{
		_vmConsole.Append(s);
		//
	}

	public void OnStep()
	{
		
	}

	public void OnRunComplete()
	{
		
	}
}