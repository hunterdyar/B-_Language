using System.Data.SqlTypes;
using System.Runtime.Serialization;
using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using BMinus.Environment;
using BMinus.Models;
using BMinus.VirtualMachine;
using VM = BMinus.VirtualMachine.VirtualMachine;
namespace BMinus.Compiler;

//todo: B allows you to declare functions above or below where you use them. and externals. which is good!
//but we can't compile in a single pass. 
//In frame 0, we need to do a pass with no recursion for variable declarations and for function declarations, and add them.
//we don't compile them yet, because recursion would still break.
//or we go through the entire compilation, and if a function is not found, we don't throw an error, we just add it to a list of operations that need an updated operand for function names.
//at the end, we go through this list, and update the operands or throw the error. that feels faster in my head.
//we will need to do this for functions, labels, and externs.

public class Compiler
{
	public Statement Root;
	//shorthands for register indices. can move to VM as static.

	//environment
	private List<string> _globals = new List<string>();

	//functions, basically
	private SubroutineDefinition Frame => _subroutines[_frames.Peek()];
	
	private readonly Dictionary<string,SubroutineDefinition> _subroutines = new Dictionary<string, SubroutineDefinition>();
	private readonly Dictionary<string, InstructionLocation> _labels = new Dictionary<string, InstructionLocation>();
	//a stack of frames is needed too? is it? how do we keep compiling the root frame after we finish the subroute
	private Stack<string> _frames = new Stack<string>();

	private VMRunner _runner;
	public Compiler(VMRunner runner)
	{
		_runner = runner;
	}
	public void NewCompile(Statement s)
	{
		_frames.Clear();
		_labels.Clear();
		_subroutines.Clear();
		_globals.Clear();
		
		Root = s;
		_subroutines.Add("", new SubroutineDefinition("", 0)); //the global frame.
		_frames.Push("");
		Compile(Root);
	}
	
	public void Compile(Statement statement)
	{
		if (statement is ProgramStatement programStatement)
		{
			foreach (var s in programStatement.Statements)
			{
				Compile(s);
			}
		}else if (statement is CompoundStatement compoundStatement)
		{
			foreach (var s in compoundStatement.Statements)
			{
				Compile(s);
			}
		}else if (statement is Assignment assignment)
		{
			var left = assignment.Identifier.Value;
			//todo: move to resolver function
			int leftID = 0;
			if (_globals.Contains(left))
			{
				leftID = _globals.IndexOf(left);
			}
			else
			{
				//check locals, etc etc, in the right order, etc.
				throw new CompilerException($"Unable to resolve variable named {left}");
			} 
			CompileExpression(assignment.ValueExpr, VM.X);
			//if isLocal
			if (Frame.FrameID == 0)
			{
				Emit(OpCode.SetGlobal, assignment.UID, leftID, VM.X);
			}
			else
			{
				Emit(OpCode.SetLocal, assignment.UID,leftID, VM.X);
			}

		}else if (statement is VariableDeclaration varDeclaration)
		{
			foreach (var identifier in varDeclaration.Identifiers)
			{
				if (Frame.FrameID == 0)
				{
					Frame.AddExtern(identifier.Value, _globals.Count);
					_globals.Add(identifier.Value);
				}
				else
				{
					Frame.AddLocal(identifier.Value);
				}
			}
		} else if(statement is ExternDeclaration externDeclaration)
		{
			foreach (var var in externDeclaration.Identifiers)
			{
				//add identifier to our variable resolution system.
				if (Frame.FrameID == 0)
				{
					throw new CompilerException($"Extern keyword is invalid at top level.");
				}

				int index = _globals.IndexOf(var.Value);
				if (index != -1)
				{
					Frame.AddExtern(var.Value, index);
				}
				else
				{
					throw new CompilerException($"Unable to resolve extern {var.Value}");
				}
			}
		}else if (statement is FunctionCall fn)
		{
			var name = fn.FunctionName.Value;
			
			//compile and put onto the stack in order. 
			foreach (var arg in fn.Arguments)
			{
				CompileExpression(arg,-1);
			}

			if (Builtins.IsBuiltin(name, out var index))
			{
				Emit(OpCode.CallBuiltin,fn.UID,index, fn.Arguments.Length);
				return;
			}

			//getFunctionID = fn.FunctionName;

			if (!_subroutines.TryGetValue(fn.FunctionName.Value, out SubroutineDefinition sub))
			{
				throw new CompilerException($"Unable to find function {fn.FunctionName}");
			}
			Emit(OpCode.Call,fn.UID, sub.FrameID);
			return;
			//entering a frame sets base stackPointer, etc.
			//after the function is done, we will return to this location of this frame.
			//Runtime frames will clean the stack when we leave them.
		}else if (statement is FunctionDeclaration fnDec)
		{
			var name = fnDec.Identifier.Value;
			if (_subroutines.ContainsKey(name))
			{
				throw new CompilerException($"A function named {name} already exists");
			}
			//set a function prototype frame ID for name
			//create a new frame

			_subroutines.Add(name,new SubroutineDefinition(name,_subroutines.Count));
			_frames.Push(name);
			//in this frame, compile the arguments into gets from the stack (eh?) into local variables (id 0,1,2,etc)
			foreach (var parameter in fnDec.Parameters)
			{
				Frame.AddLocal(parameter.Value);
			}
			
			Compile(fnDec.Statement);
			Emit(OpCode.Return, fnDec.UID);//Leaves the frame, (which cleans the stack from it's locals). With a value perhaps in EAX.
			_frames.Pop();
			return;
		}else if(statement is GoTo gotoStatement)
		{
			//todo: a temporary instruction type would be fine. THen we finish compiling, and we search for the label, etc.
			//dynamic goto's are not supported, this is compile time.
			// string label = temporary LabelValue.
			//destination = getdestination
			//if we are not in the current frame, we need to clean the stack?
			//shit.
			Emit(OpCode.GoTo, gotoStatement.UID,0, 0);//
			return;
		}else if (statement is IfElseStatement ifElseStatement)
		{
			//note we check if/else before checking the bare if.
			CompileExpression(ifElseStatement.Condition, VM.X);
			var jumpCons = Emit(OpCode.JumpZero, ifElseStatement.UID, 9999, 9999);
			
			//these two are the consequence together.
			Compile(ifElseStatement.Consequence);
			var jumpAlt = Emit(OpCode.Jump, ifElseStatement.UID,9999, 9999);
			
			var prealt = Frame.GetTopInstructionLocation();
			
			//this is the alt
			Compile(ifElseStatement.Alternative);
			
			var end = Frame.GetTopInstructionLocation();
			UpdateOperands(jumpCons, prealt.FrameIndex, prealt.InstructionIndex);
			UpdateOperands(jumpAlt, end.FrameIndex, end.InstructionIndex);
		}else if (statement is IfStatement ifStatement)
		{
			CompileExpression(ifStatement.Condition,VM.X);
			var jnz = Emit(OpCode.JumpZero, ifStatement.UID,9999,9999);
			Compile(ifStatement.Consequence);
			var top = Frame.GetTopInstructionLocation();
			UpdateOperands(jnz, top.FrameIndex, top.InstructionIndex);
		}else if (statement is Label label)
		{
			//Create label
			_labels.Add(label.LabelID,Frame.GetTopInstructionLocation());
		}else if (statement is Nop nop)
		{
			Emit(OpCode.Nop, nop.UID);
			return;
		}
	}

	/// <summary>
	/// Compiles an expression and emits instructions to put it on the stack.
	/// </summary>
	/// <param name="expression">Generate a value that that gets put into a register or onto the stack.</param>
	/// <param name="register">which register, or -1 for on the stack.</param>
	public void CompileExpression(Expression expression, int register = 0)
	{
		if(expression is WordLiteral wordLiteral)
		{
			Emit(OpCode.SetReg, expression.UID,wordLiteral.ValueAsInt, register);
			//add to register.
			//add instruction, push wordLiteral.GetValue.
		}else if (expression is StringLiteral stringLiteral)
		{
			//strings are stored on the heap and accessed through pointers.
			//so we need to do pointers+arrays before we do strings
			
			//push each character of the array on the stack,
			//set each consecutive position of memory with the characters.
			//ends with a pointer to the location of the character.
		}else if (expression is BinMathOp binMathOp)
		{
			CompileExpression(binMathOp.Left, VM.A);
			CompileExpression(binMathOp.Right, VM.B);
			Emit(OpCode.Arithmetic, binMathOp.UID,(int)binMathOp.Op, register);//leaves result in EAX
		}else if (expression is CompareOp compareOp)
		{
			CompileExpression(compareOp.Left, VM.A);
			CompileExpression(compareOp.Right, VM.B);
			Emit(OpCode.Compare, compareOp.UID,(int)compareOp.Op, register);
		}else if (expression is TernaryOp ternary)
		{
			//ternary's are if's, except they leave a value in the register.
			CompileExpression(ternary.Condition, VM.X);
			var j_nq = Emit(OpCode.JumpNotZero, expression.UID,0);
			CompileExpression(ternary.Consequence, VM.A);
			var la = TopLocation();
			var j_skipAlt = Emit(OpCode.Jump, expression.UID);
			CompileExpression(ternary.Alternative, VM.B);
			var lb = TopLocation();
			UpdateOperands(j_nq, j_skipAlt.FrameIndex, j_skipAlt.InstructionIndex);//+1?
			UpdateOperands(j_skipAlt, lb.FrameIndex, lb.InstructionIndex);

			//get position?
		}else if (expression is Identifier identifier)
		{
			if (Frame.TryResolveID(identifier.Value, out int index, out Scope scope))
			{
				if (scope == Scope.Local || scope == Scope.Argument)
				{
					Emit(OpCode.GetLocal, expression.UID, index, register);
				}else if (scope == Scope.Global)
				{
					Emit(OpCode.GetGlobal, expression.UID, index, register);
				}
				return;
			}

			throw new CompilerException($"Unble to resolve variable {identifier.Value}. Has it been declared?");
		}
	}
	
	#region Helpers
	private InstructionLocation Emit(OpCode code, uint astNodeID, params int[] operands)
	{
		if (operands.Length == 0)
		{
			return Frame.AddInstruction(new Instruction(code, astNodeID));
		}else if (operands.Length == 1)
		{
			return Frame.AddInstruction(new Instruction(code, astNodeID, operands[0]));
		}
		else if (operands.Length == 2)
		{
			return Frame.AddInstruction(new Instruction(code, astNodeID, operands[0], operands[1]));
		}else
		{
			throw new CompilerException("Too many operands");
		}
	}

	private SubroutineDefinition GetFrame(string name)
	{
		return _subroutines[name];
	}
	
	private InstructionLocation TopLocation()
	{
		return Frame.GetTopInstructionLocation();
	}

	private void UpdateOperands(InstructionLocation original, params int[] newOps)
	{
		var kvp = _subroutines.First(x => x.Value.FrameID == original.FrameIndex);
		var f = kvp.Value;
		f.UpdateOperands(original,newOps);
	}
	
	#endregion

	public Environment.Environment GetEnvironment()
	{
		return new Environment.Environment(_runner,Root, _globals,GetFrames());
	}

	//todo: move this to environment, clone at runtime...
	public Frame[] GetFrames()
	{
		var frames = new Frame[_subroutines.Count];
		foreach (var subroutine in _subroutines)
		{
			frames[subroutine.Value.FrameID] = new Frame(subroutine.Value);
		}

		if (frames.Any(x => x == null))
		{
			throw new CompilerException("Multiple functions with same name?");
		}
		
		return frames;
	}
	
}