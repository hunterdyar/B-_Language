using System;
using System.Collections.Generic;
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
		_runner.OnStateChange += (s) => OnStatehange((int)s);
		_runner.OnErrorThrow += OnErrorThrown;
		_runner.OnEnterNewFrame += OnEnterNewFrame;
		_runner.OnFramePop += OnFramePop;
	}

	[JSExport]
	public static void RunProgram(string program)
	{
		var output = _runner.RunProgram(program);
		SendOutput(output);
	}

	[JSExport]

	public static bool Compile(string program)
	{
		return _runner.Compile(program);
	}

	[JSExport]
	public static void Step()
	{
		_runner.Step();
	}
	
	[JSExport]
	public static int GetState()
	{
		return (int)_runner.VMState;
	}

	[JSExport]
	public static int GetFrameCount()
	{
		return _runner.Env.FramePrototypeCount;
	}
	
	[JSExport]
	public static string[] GetInstructions(int f)
	{
		var frame = _runner.Env.GetFramePrototype(f);
		var instructions = new string[frame.Instructions.Count*5];
		for (int i = 0; i < frame.Instructions.Count; i++)
		{
			var ins = InstructionToStringArray(frame.Instructions[i]);
			int j = i * 5;
			instructions[j] = ins[0];
			instructions[j+1] = ins[1];
			instructions[j+2] = ins[2];
			instructions[j+3] = ins[3];
			instructions[j+4] = ins[4];
		}

		return instructions;
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


	public static void OnInstructionChange(Instruction ins, (int f, int l) loc)
	{
		SendInstruction(InstructionToStringArray(ins, loc.f,loc.l));
	}
	
	private static string[] InstructionToStringArray(Instruction ins, int f=0, int l=0)
	{
		string a = ins.OperandA.ToString();
		string b = ins.OperandB.ToString();
		switch (ins.Op)
		{
			case OpCode.Arithmetic:
				a = ((BinaryArithOp)ins.OperandA).ToString();
				b = VirtualMachine.RegisterName(ins.OperandB);
				break;
			case OpCode.Compare:
				a = ((Comparison)ins.OperandA).ToString();
				b = VirtualMachine.RegisterName(ins.OperandB);
				break;
			case OpCode.Return:
				b = VirtualMachine.RegisterName(ins.OperandB);
				break;
			case OpCode.CallBuiltin:
				a = Builtins.GetBuiltinName(ins.OperandA);
				break;
			case OpCode.SetReg:
				b = VirtualMachine.RegisterName(ins.OperandB);
				break;
			
			case OpCode.GetGlobal:
			case OpCode.SetGlobal:
			case OpCode.SetLocal:
			case OpCode.GetLocal:
				b = VirtualMachine.RegisterName(ins.OperandB);
				break;
			case OpCode.Halt:
			case OpCode.Pop:
			case OpCode.Nop:
				a = b = "";
				break;
		}

		return new[] { ins.Op.ToString(), a, b, ins.ASTNodeID.ToString(),f.ToString(),l.ToString()};
	}
	
	[JSImport("onInstruction", "main.js")]
	public static partial void SendInstruction(string[] ins);

	[JSImport("onStack", "main.js")]
	public static partial void OnStackChange(int[] stack, int totalSize);

	[JSImport("onState", "main.js")]
	public static partial void OnStatehange(int state);
	
	[JSImport("onError", "main.js")]
	public static partial void OnErrorThrown(string errorType, string errorMessage);

	[JSImport("onHeapValue", "main.js")]
	public static partial void OnValueUpdated(int frame, int loc, byte[] newVal);

	[JSExport]
	public static byte[] GetHeapMemory()
	{
		return _runner.Env.HeapMemorySegment();
	}

	private static void OnEnterNewFrame(Frame frame)
	{
		OnFrameEnter(frame.Source.Name,frame.FrameID,frame.Source.Locals.Keys.ToArray());
	}

	[JSImport("onFrameEnter", "main.js")]
	public static partial void OnFrameEnter(string fname, int fid, string[] locals);

	[JSImport("onFramePop","main.js")]
	public static partial void OnFramePop();
}