using System.Diagnostics;
using System.Text;
using BMinus.Compiler;
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
	public Action<int[]> OnRegistersChange { get; set; }
	public Action<Instruction> OnCurrentInstructionChange { get; set; }
	public Action<int[],int> OnStackChange { get; set; }

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
			var c =Compile(program);
			if (!c)
			{
				Console.WriteLine("Compile Error");
				return "Compiler Error";
			}
			_vm.Run();
			_runtimeWatch.Stop();
			if (report)
			{
				_vmConsole.Append("\n");
				_vmConsole.AppendLine($"---\nB- Execution Finished in {_runtimeWatch.ElapsedMilliseconds}ms");
			}

			string o = _vmConsole.ToString();
			OnOutputChange?.Invoke(o);
			OnRegistersChange?.Invoke(_vm.Register);
			return VMConsole.ToString();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return e.Message;
		}
	}

	public bool Compile(string program)
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
			return true;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			_vmConsole.Append(e.Message);
			OnOutputChange?.Invoke(e.Message);
			return false;
		}

		return false;
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
		if (_vm.RegisterDirty)
		{
			OnRegistersChange?.Invoke(_vm.Register);
		}

		if (_vm.StackDirty)
		{
			OnStackChange?.Invoke(_vm.GetStackArray(10),_vm.CurrentStackSize);
		}
		OnCurrentInstructionChange?.Invoke(_vm.CurrentInstrution);
		_vm.Flush();
	}

	public void OnRunComplete()
	{
		OnOutputChange?.Invoke(_vmConsole.ToString());
		OnCurrentInstructionChange?.Invoke(_vm.CurrentInstrution);
	}
}