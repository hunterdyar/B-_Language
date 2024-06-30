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
	private Stopwatch _runtimeWatch = new Stopwatch();

	public StringBuilder VMConsole => _vmConsole;
	private StringBuilder _vmConsole = new StringBuilder();
	public VMState VMState => GetVMState();
	public Action<string> OnOutputChange { get; set; }

	private VMState GetVMState()
	{
		if (_vm == null)
		{
			return VMState.Uninitialized;
		}
		else
		{
			return _vm.State;
		}
	}

	//todo: move measuring and reporting to the runner.
	public string RunProgram(string program, bool report = false)
	{
		_vmConsole.Clear();
		try
		{
			Compile(program);
			_vm.Run();
			_runtimeWatch.Stop();
			if (report)
			{
				_vmConsole.Append("\n");
				_vmConsole.AppendLine($"---\nB- Execution Finished in {_runtimeWatch.ElapsedMilliseconds}ms");
			}

			string o = _vmConsole.ToString();
			OnOutputChange?.Invoke(o);
			return VMConsole.ToString();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return e.Message;
		}
	}

	public void Compile(string program)
	{
		_vmConsole.Clear();
		try
		{
			_compileWatch.Restart();
			_parser = new Parser.Parser(new Lexer(program));
			var tree = _parser.Parse();
			_compiler.NewCompile(tree);
			_compileWatch.Stop();
			var env = _compiler.GetEnvironment();
			_vm = new VirtualMachine(env, this);
			_env = _vm.Env;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			_vmConsole.Append(e.Message);
		}
	}
	public void Step()
	{
		if (_vm != null)
		{
			_vm.StepOver();

		}
		else
		{
			Console.WriteLine($"Error stepping. no program compiled.");
		}
	}
	public void Append(string s)
	{
		_vmConsole.Append(s);
		OnOutputChange?.Invoke(_vmConsole.ToString());
	}

	public void OnStep()
	{
		
	}

	public void OnRunComplete()
	{
		_vmConsole.AppendLine("complete!");
		OnOutputChange?.Invoke(_vmConsole.ToString());
	}
}