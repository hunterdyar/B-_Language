using System;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using BMinus.Compiler;
using BMinus.Environment;
using BMinus.Models;
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
		_runner.OnRegistersChange += SendRegisters;
		_runner.OnCurrentInstructionChange += OnInstructionChange;
		_runner.OnStackChange += OnStackChange;
	}

	

	[JSExport]
	public static void RunProgram(string program)
	{
		var output = _runner.RunProgram(program);
		SendOutput(output);
	}

	[JSExport]

	public static void Compile(string program)
	{
		Console.WriteLine("Compiling");
		_runner.Compile(program);
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

	[JSExport]
	public static string GetAST()
	{
		return _runner.Env.AST.GetJSON();
	}
	
	[JSImport("onOutput", "main.js")]
	public static partial void SendOutput(string newLine);

	[JSImport("onRegister", "main.js")]
	public static partial void SendRegisters(int[] registers);


	public static void OnInstructionChange(Instruction ins)
	{
		string a = ins.OperandA.ToString();
		string b = ins.OperandB.ToString();
		int opCount = 2;
		switch (ins.Op)
		{
			case OpCode.Arithmetic:
				a = ((BinaryArithOp)ins.OperandA).ToString();
				opCount = 1;
				break;
			case OpCode.Compare:
				a = ((Comparison)ins.OperandA).ToString();
				opCount = 1;
				break;
			case OpCode.CallBuiltin:
				a = Builtins.GetBuiltinName(ins.OperandA);
				opCount = 2;
				break;
			case OpCode.SetReg:
				b = VirtualMachine.RegisterName(ins.OperandB);
				break;
			case OpCode.Nop:
				opCount = 0;
				break;
			case OpCode.GetGlobal:
			case OpCode.SetGlobal:
			case OpCode.SetLocal:
			case OpCode.GetLocal:
				b = VirtualMachine.RegisterName(ins.OperandB);
				break;
			case OpCode.Pop:
				opCount = 0;
				break;
		}
		
		SendInstruction(new []{ins.Op.ToString(),a,b},(int)ins.ASTNodeID,opCount);
	}
	
	[JSImport("onInstruction", "main.js")]
	public static partial void SendInstruction(string[] ins, int id, int operands);

	[JSImport("onStack", "main.js")]
	public static partial void OnStackChange(int[] stack, int totalSize);
	
}