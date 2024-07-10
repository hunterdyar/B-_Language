using System.Diagnostics;
using BMinus.Compiler;
using BMinus.Environment;
using BMinus.Models;

namespace BMinus.VirtualMachine;
using Environment = Environment.Environment;
public class VirtualMachine
{
	public static readonly int X = 0; //accumulator, result of operations.
	public static readonly int A = 1; //general
	public static readonly int B = 2; //general
	public static readonly int RET = 6;
	public static readonly int A2 = 3; //
	public static readonly int B2 = 4;
	public static readonly int X2 = 5;
	public static readonly int S = -1;
	public const int StackSize = 2048;
	
	public VMState State => _state;
	private VMState _state = VMState.Ready;
	public Environment Env;
	public VMRunner Runner => _runner;
	private VMRunner _runner;

	public Instruction CurrentInstruction => _currentInstruction;
	private Instruction _currentInstruction = new Instruction(OpCode.Nop);
	public (int, int) CurrentInstrutionLocation = (0,0);
	public uint CurrentASTNodeID => _currentASTNodeID;
	private uint _currentASTNodeID;
	//Compiler gives us a compiler object, which is NOT really bytecode,
	//as we will have initial heap state (constants) and frame prototypes as objects.
	public int[] Register => _register;
	private int[] _register;
	private int[] _stack;
	/// <summary>
	/// Stack Pointer. Points to next element.
	/// </summary>
	private int _sp = 0;
	/// <summary>
	/// Frame Pointer
	/// </summary>
	private int fp = 0;

	public bool StackDirty => _stackDirty;
	private bool _stackDirty;
	public bool RegisterDirty => _registerDirty;
	private bool _registerDirty;
	private readonly Stopwatch _stopwatch = new Stopwatch();
	/// <summary>
	/// Instruction Pointer
	/// </summary>
	private int ip => CurrentFrame.IP;

	private Stack<Frame> _frames;
	private Frame CurrentFrame => _frames.Peek();
	public int CurrentStackSize => _sp;

	public VirtualMachine(Environment env, VMRunner? runner = null)
	{
		if (runner == null)
		{
			_runner = new VMRunner();
		}
		else
		{
			_runner = runner;
		}

		this.Env = env;
		_register = new int[8];
		_stack = new int[StackSize];
		_sp = 0;
		_frames = new Stack<Frame>();
		_frames.Push(env.GetFramePrototype(0));
		SetState(VMState.Ready);
		_runner.OnFrameEnter(1,_frames.Peek());
		_registerDirty = false;
		_stackDirty = false;
	}

	private void SetState(VMState state)
	{
		if (_state != state)
		{
			_state = state;
		}
		else
		{
			return;
		}

		_runner.OnState(_state);
	}

	public void Run()
	{
		_stopwatch.Restart();
		//ready, or partway stepping (now should resume and run to end).
		if (_state == VMState.Ready || _state == VMState.Stepping)
		{
			SetState(VMState.Running);
		}
		else
		{
			throw new VMException($"Can't run state, state is {_state}");
		}
		
		while (_state == VMState.Running)
		{
			RunOne();
		}
		
		_stopwatch.Stop();
		_runner.OnRunComplete();
	}

	public void StepOver()
	{
		if (_state != VMState.Stepping)
		{
			if (_state == VMState.Ready)
			{
				SetState(VMState.Stepping);
			}else if (_state == VMState.Complete)
			{
				_runner.OnRunComplete();
				return;
			}
			else
			{
				throw new VMException($"Can't step, state is {_state}");
			}
		}
		
		RunOne();
		_runner.OnStep();
	}

	public void Flush()
	{
		_registerDirty = false;
		_stackDirty = false;
	}
	
	private void RunOne()
	{
		if (_state != VMState.Running && _state != VMState.Stepping)
		{
			return;
		}
		CurrentFrame.IP++;
		if (CurrentFrame.IP >= CurrentFrame.Instructions.Count)
		{
			if (_frames.Count > 0)
			{
				LeaveFrame();
			}
			else
			{
				SetState(VMState.Complete);
			}
			return;
		}

		var op = CurrentFrame.Instructions[CurrentFrame.IP];
		_currentInstruction = op;
		//todo: I think dynamic acess is getting messed up by the Return (current frame changes), so hard-coding it. now other things are wonking out.
		CurrentInstrutionLocation = (_frames.Count - 1, CurrentFrame.IP);
		_currentASTNodeID = op.ASTNodeID;
		switch (op.Op)
		{
			case OpCode.Nop:
				return;
			case OpCode.SetReg:
				SetRegister(op.OperandB, op.OperandA);
				return;
			case OpCode.GetGlobal:
				SetRegister(op.OperandB, Env.GetGlobal(op.OperandA));
			return;
			case OpCode.SetGlobal:
				Env.SetGlobal(op.OperandA, GetRegister(op.OperandB));
				return;
			case OpCode.GetLocal:
				int stackPos = CurrentFrame.StackBasePos + op.OperandA;
				SetRegister(op.OperandB,_stack[stackPos]);
				return;
			case OpCode.SetLocal:
				stackPos = CurrentFrame.StackBasePos + op.OperandA;
				_stack[stackPos] = GetRegister(op.OperandB);
				return;
			case OpCode.Arithmetic:
				int result = DoArithmetic(_register[A], _register[B], (BinaryArithOp)op.OperandA);
				SetRegister(op.OperandB, result);
				return;
			case OpCode.Compare:
				result = DoCompare(_register[A], _register[B], (Comparison)op.OperandA);
				SetRegister(op.OperandB, result);
				return;
			case OpCode.Bitwise:
				SetState(VMState.Error);
				throw new NotImplementedException("Bitwise not implemented in VM.");
				return;
			case OpCode.SaveRegister:
				//todo: this doesn't work because we add to the stack before calling OpCode.Call.
				SetRegister(S, _register[X]);
				SetRegister(S, _register[A]);
				SetRegister(S, _register[B]);
				Console.WriteLine("push reg");
				return;
			case OpCode.RestoreRegister:
				_register[B] = Pop();
				_register[A] = Pop();
				return;
			case OpCode.Call:
				//todo: rename GetFramePrototype here. should create frame from that instead of cloning.
				var prototype = Env.GetFramePrototype(op.OperandA);
				var f = prototype.Clone();//todo: this was written before i split runtime and compile time (frame/subroutine definition. keep subroutines instead of frames in VM)
				f.SetBasePointer(_sp-f.ArgCount);
				f.ReturnRegister = op.OperandB;
				//this creates room for arguments, but not for locals? should they just get pushed to stack?
				//_sp += f.LocalVarCount-f.ArgCount;//now the stack has room for locals too, should anything else use the stack.
				_frames.Push(f);
				_runner.OnFrameEnter(_frames.Count, f);

				//save the current register data before beginning the call.
				return;
			case OpCode.CallBuiltin:
				var builtin = op.OperandA;
				var argCount = op.OperandB;
				var args = new int[argCount];
				for (int i = argCount - 1; i >= 0; i--)
				{
					args[i] = Pop();
				}
				Builtins.CallBuiltin(this,builtin,args);
				break;
			case OpCode.Halt:
				return;
			case OpCode.Jump:
				//todo: jumps in/out of frame? should remove frame from jumps i guess.
				CurrentFrame.SetIP(op.OperandB);
				return;
			case OpCode.JumpNotZero:
				var frame = op.OperandA;
				var ip = op.OperandB;
				var condition = GetRegister(X);
				if (condition != 0)
				{
					if (frame != CurrentFrame.FrameID)
					{
						throw new VMException("Can't JNZ out of current frame.");
					}
					
					CurrentFrame.SetIP(ip);
				}
				return;
			case OpCode.JumpZero:
				frame = op.OperandA;
				ip = op.OperandB;
				condition = GetRegister(X);
				if (condition == 0)
				{
					if (frame != CurrentFrame.FrameID)
					{
						throw new VMException("Can't JNZ out of current frame.");
					}

					CurrentFrame.SetIP(ip);
				}

				return;
			case OpCode.GoTo: 
				frame = op.OperandA;
				ip = op.OperandB;
				if (frame == _frames.Count - 1)
				{
					CurrentFrame.SetIP(ip);
				}else if (frame > _frames.Count)
				{
					throw new VMException("Can't GoTo inner function, can exit functions (frames)");
				}

				while (frame < _frames.Count)
				{
					LeaveFrame();
				}
				//
				return;
			case OpCode.Return:
				SetRegister(RET, GetRegister(op.OperandA));
				LeaveFrame();
				return;
			case OpCode.Pop:
				SetRegister(X,Pop());
				return;
			case OpCode.Move:
				SetRegister(op.OperandB,GetRegister(op.OperandA));
				return;
		}
	}

	private int Pop()
	{
		return GetRegister(S);
	}

	private void Push(int val)
	{
		SetRegister(S, val);
	}
	private void LeaveFrame()
	{
		var p = _frames.TryPop(out var f);
		if (p)
		{
			//clear the stack
			for (int i = 0; i < f.ArgCount; i++)
			{
				Pop();
			}

			_runner.OnFrameExit(_frames.Count);

			//we're done!
			if (_frames.Count == 0)
			{
				SetState(VMState.Complete);
				return;
			}

		}
		else
		{
			SetState(VMState.Error);
			throw new Exception("Unable to leave frame");
		}
	}

	private void SetRegister(int reg, int val)
	{
		if (reg < 0)
		{
			if (_sp >= _stack.Length)
			{
				SetState(VMState.Error);
				throw new VMException("Stack Overflow!");
			}
			_stack[_sp] = val;
			_sp++;
			_stackDirty = true;
			return;
		}

		_register[reg] = val;
		_registerDirty = true;
	}

	private int GetRegister(int reg)
	{
		if (reg < 0)
		{
			_sp--;
			_stackDirty = true;
			if (_sp < 0)
			{
				throw new VMException("SHIT");
			}
			return _stack[_sp];
		}

		return _register[reg];
	}
	private int DoCompare(int l, int r, Comparison op)
	{
		switch (op)
		{
			case Comparison.Equals:
				return l == r ? 1 : 0;
			case Comparison.NotEquals:
				return l != r ? 1 : 0;
			case Comparison.GreaterThan:
				return l > r ? 1 : 0;
			case Comparison.LessThan:
				return l < r ? 1 : 0;
			case Comparison.GreaterThanOrEqual:
				return l >= r ? 1 : 0;
			case Comparison.LessThanOrEqual:
				return l <= r ? 1 : 0;
		}

		throw new VMException($"Bad comparison operator {op}");
	}

	private int DoArithmetic(int l, int r, BinaryArithOp op)
	{
		switch (op)
		{
			case BinaryArithOp.Add:
				return l + r;
			case BinaryArithOp.Subtract:
				return l - r;
			case BinaryArithOp.Multiply:
				return l * r;
			case BinaryArithOp.Divide:
				return l / r;
			case BinaryArithOp.Remainder:
				return l % r;
		}

		throw new VMException($"Bad arithmetic operator {op}");

	}

	public static string RegisterName(int reg)
	{
		if (reg < 0)
		{
			return "Stack";
		}
		return new[]{"X", "A", "B", "A2", "B2", "X2", "RET"}[reg];
	}

	public int[] GetStackArray(int max)
	{
		int size = _sp > max ? max : _sp;
		return new ArraySegment<int>(_stack, _sp-size, size).Reverse().ToArray();
	}
}