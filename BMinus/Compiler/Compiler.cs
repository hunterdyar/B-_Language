﻿using System.Data.SqlTypes;
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
	//todo: i don't like that this is static, would rather inject reference to compiler instance.
	public static Action<int, int> OnInstructionRemoved;
	public Statement Root;
		//environment
	public Dictionary<string, int> Globals => _globals;
	private Dictionary<string, int> _globals = new Dictionary<string, int>();

	private List<UnknownFunctionCall> _unknownFunctionCalls = new List<UnknownFunctionCall>();

	private HashSet<UnknownExtern> _unknownExterns = new HashSet<UnknownExtern>();

	private List<UnknownGoTo> _unknownGoTos = new List<UnknownGoTo>();
	//functions, basically
	private SubroutineDefinition Frame => _subroutines[_frames.Peek()];
	public Dictionary<string, SubroutineDefinition> Subroutines => _subroutines;

	private readonly Dictionary<string,SubroutineDefinition> _subroutines = new Dictionary<string, SubroutineDefinition>();
	public Dictionary<string, InstructionLocation> Labels => _labels;
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
		_unknownFunctionCalls.Clear();
		_unknownExterns.Clear();
		_frames.Clear();
		_labels.Clear();
		_subroutines.Clear();
		_globals.Clear();
		
		Root = s;
		_subroutines.Add("", new SubroutineDefinition("", 0,0)); //the global frame.
		_frames.Push("");
		Compile(Root);

		foreach (var unknownFn in _unknownFunctionCalls)
		{
			unknownFn.TryToFindCallAgain(this);
		}

		foreach (var unknownExtern in _unknownExterns)
		{
			unknownExtern.TryToFindExternAgain(this);
		}

		foreach (var uGoTo in _unknownGoTos)
		{
			// uGoTo
		} 
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
			(int id, Scope s) = ResolveVariable(left);
			
			CompileExpression(assignment.ValueExpr, VM.X);
			//if isLocal
			if (s == Scope.Global)
			{
				Emit(OpCode.SetGlobal, assignment.UID, id, VM.X);
			}
			else if(s == Scope.Local)
			{
				Emit(OpCode.SetLocal, assignment.UID,id, VM.X);
			}else if (s == Scope.UnknownGlobal)
			{
				var save = Emit(OpCode.SetGlobal, assignment.UID, 9999, VM.X);
				_unknownExterns.Add(new UnknownExtern(left, VM.X, save, Frame));
			}
			else
			{
				throw new CompilerException("Bad variable scope. Also, this error should have been caught before here. If you are reading this, sorry. Something went wrong.");
			}

		}else if (statement is VariableDeclaration varDeclaration)
		{
			foreach (var identifier in varDeclaration.Identifiers)
			{
				if (Frame.FrameID == 0)
				{
					int nextGlobalAddress = _globals.Count * 4;
					Frame.AddExtern(identifier.Value, nextGlobalAddress);
					_globals.Add(identifier.Value, nextGlobalAddress);
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
				
				Frame.AddUnknownExtern(var.Value);
			}
		}else if (statement is FunctionCall fn)
		{
			CompileFunctionCall(fn, true);
		}else if (statement is FunctionDeclaration fnDec)
		{
			var name = fnDec.Identifier.Value;
			if (_subroutines.ContainsKey(name))
			{
				throw new CompilerException($"A function named {name} already exists");
			}
			//set a function prototype frame ID for name
			//create a new frame

			_subroutines.Add(name,new SubroutineDefinition(name,_subroutines.Count, fnDec.Parameters.Length));
			_frames.Push(name);
			
			//Save the existing value of registers, to prevent clobbering.
			// if (ShouldSaveRegisters())
			// {
			// 	Emit(OpCode.SaveRegister, fnDec.UID);
			// }
			//in this frame, compile the arguments into gets from the stack (eh?) into local variables (id 0,1,2,etc)

			foreach (var parameter in fnDec.Parameters)
			{
				Frame.AddLocal(parameter.Value);
			}

			Compile(fnDec.Statement);

			
			if (Frame.Instructions.Count == 0 || Frame.LastInstruction.Op != OpCode.Return)
			{
				// if (ShouldSaveRegisters())
				// {
				// 	Emit(OpCode.RestoreRegister, fnDec.UID);
				// }
				Emit(OpCode.Return, fnDec.UID, VM.X); //Leaves the frame, (which cleans the stack from it's locals). With a value perhaps in EAX.
			}

			_frames.Pop();
			return;
		}else if(statement is GoTo gotoStatement)
		{
			//todo: a temporary instruction type would be fine. THen we finish compiling, and we search for the label, etc.
			//dynamic goto's are not supported, this is compile time.
			// string label = temporary LabelValue.
			//destination = getdestination
			if (_labels.TryGetValue(gotoStatement.Label, out var location))
			{
				//the VM should handle the frameIndex, and repeatedly calling LeaveFrame.
				Emit(OpCode.GoTo, gotoStatement.UID,location.FrameIndex, location.FrameIndex);//
			}
			else
			{
				//save this until we finish the rest of compiling.
				var gotoLoc = Emit(OpCode.GoTo, gotoStatement.UID,999, 999);//
				_unknownGoTos.Add(new UnknownGoTo(gotoStatement.Label,gotoLoc));
			}
			//shit.
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
		}else if (statement is WhileLoop whileLoop)
		{
			CompileExpression(whileLoop.Condition, VM.X);
			var start = Frame.GetTopInstructionLocation();
			var jnz = Emit(OpCode.JumpZero, whileLoop.UID, 9999, 9999);
			Compile(whileLoop.Consequence);
			Emit(OpCode.Jump, whileLoop.UID, start.FrameIndex, start.InstructionIndex);
			var end = Frame.GetTopInstructionLocation();
			UpdateOperands(jnz, end.FrameIndex, end.InstructionIndex);
		}else if (statement is Label label)
		{
			//Create label
			_labels.Add(label.LabelID,Frame.GetTopInstructionLocation());
		}else if (statement is Nop nop)
		{
			Emit(OpCode.Nop, nop.UID);
			return;
		}else if (statement is ReturnStatement returnStatement)
		{
			if (returnStatement.Value != null)
			{
				CompileExpression(returnStatement.Value,VM.X);
			}

			// if (ShouldSaveRegisters())
			// {
			// 	Emit(OpCode.RestoreRegister, returnStatement.UID);
			// }
			Emit(OpCode.Return, returnStatement.UID, VM.X);
		}
	}

	private (int id, Scope s) ResolveVariable(string varName)
	{
		if (_subroutines.ContainsKey(varName))
		{
			throw new CompilerException($"Can't resolve variable name {varName}, {varName} has been defined as a function.");
		}
		if (Frame.TryGetLocal(varName, out var id))
		{
			return (id, Scope.Local);
		}else if(Frame.UnknownExterns.Contains(varName))
		{
			return (9999, Scope.UnknownGlobal);
		}else if (_globals.TryGetValue(varName, out id))
		{
			//todo: move this to GetAndMarkUsed in memory manager, or whathaveyou.
			//todo: consolidate logic for this and 
			return (id, Scope.Global);
		}

		throw new CompilerException($"Unknown variable {varName}.");
		return (-1, Scope.None);
	}


	/// <summary>
	/// Compiles an expression and emits instructions to put it on the stack.
	/// </summary>
	/// <param name="expression">Generate a value that that gets put into a register or onto the stack.</param>
	/// <param name="register">which register, or -1 for on the stack.</param>
	public void CompileExpression(Expression expression, int register = 0)
	{
		if (register >= 0)
		{
			Frame.ModifiedRegisters[register] = true;
		}

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
			//wait, didn't I write 
			if (Frame.TryResolveID(identifier.Value, out int index, out Scope scope))
			{
				//todo: is this the right order?
				if (scope == Scope.Local || scope == Scope.Argument)
				{
					Emit(OpCode.GetLocal, expression.UID, index, register);
				}else if (scope == Scope.Global)
				{
					Emit(OpCode.GetGlobal, expression.UID, index, register);
				}else if (scope == Scope.UnknownGlobal)
				{
					var location = Emit(OpCode.GetGlobal, expression.UID, index, register);
					_unknownExterns.Add(new UnknownExtern(identifier.Value, register, location, Frame));
				}
				
				return;
			}

			if (Frame.FrameID != 0)
			{
				if (_globals.ContainsKey(identifier.Value))
				{
					throw new CompilerException(
						$"External Variables must be declared in functions. Consider adding 'extern {identifier.Value};' to the top of the {Frame.Name} function.");
				}
			}

			throw new CompilerException($"Unble to resolve variable {identifier.Value}. Has it been declared?");
		}else if (expression is FunctionCall fn)
		{
			//try callER svae instead of callEE save.
			InstructionLocation? save = null;
			if (ShouldSaveRegisters())
			{
				save = Emit(OpCode.SaveRegister, fn.UID);
			}
			var call = CompileFunctionCall(fn, false);//clobbers A and B, X is considered clobberable, and RET should have a new value now.
			//var call = Frame.GetTopInstructionLocation();
			
			//todo: somehow need to check this during the 'unknown' step.  
			if (_subroutines.TryGetValue(fn.FunctionName.Value, out var sub))
			{
				//all normal here, add the restore, or remove the save. (either we have restore/save, or neither, as needed).
				var dirty = sub.ModifiedRegisters[VM.A] || sub.ModifiedRegisters[VM.B];
				if (dirty) //registers the caller contract says can't be modified (a or b) have been modified.
				{
					if (ShouldSaveRegisters()) //does it even matter if it's dirty.
					{
						Emit(OpCode.RestoreRegister, fn.UID);
					}
				}
				else
				{
					Frame.RemoveInstruction(save);
				}
			}
			else
			{
				//Not normal! we need to go back and fix it later, if we find the function definition.
				//we create the restore. use the save, and the call was created with a garbage function value in CompilieFunctionCall()
				InstructionLocation? restore = null;
				if (ShouldSaveRegisters())
				{ 
					restore = Emit(OpCode.RestoreRegister, fn.UID);
				}

				_unknownFunctionCalls.Add(new UnknownFunctionCall(call,fn.FunctionName.Value,Frame,save,restore));
			}

			Emit(OpCode.Move, fn.UID, VM.RET, register);
			Frame.ModifiedRegisters[VM.RET] = true;
		}
	}
	
	//called by both compileStatement and compileExpression
	private InstructionLocation CompileFunctionCall(FunctionCall fn, bool emitUnknown)
	{
		var name = fn.FunctionName.Value;
		
		//compile and put onto the stack in order. 
		foreach (var arg in fn.Arguments)
		{
			CompileExpression(arg, -1);
		}

		if (Builtins.IsBuiltin(name, out var index))
		{
			 var e =Emit(OpCode.CallBuiltin, fn.UID, index, fn.Arguments.Length);
			 return e;
		}

		//getFunctionID = fn.FunctionName;
		InstructionLocation call;
		if (!_subroutines.TryGetValue(fn.FunctionName.Value, out SubroutineDefinition? sub))
		{
			call = Emit(OpCode.Call, fn.UID, 99999, VM.RET);
			if (emitUnknown)
			{
				_unknownFunctionCalls.Add(new UnknownFunctionCall(call, fn.FunctionName.Value, Frame));
			}
		}
		else
		{
			call = Emit(OpCode.Call, fn.UID, sub.FrameID, VM.RET);
		}

		Frame.ModifiedRegisters[VM.RET] = true;
		// if (register != VM.X)
		// {
		// 	Emit(OpCode.Move,fn.UID,VM.X, register);
		// }
		return call;
		//entering a frame sets base stackPointer, etc.
		//after the function is done, we will return to this location of this frame.
		//Runtime frames will clean the stack when we leave them.
	}

	
	#region Helpers
	private InstructionLocation Emit(OpCode code, uint astNodeID, params int[] operands)
	{
		TestEmit(code, operands);
		
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
	
	private void TestEmit(OpCode op, params int[] operands)
	{
		if (op == OpCode.Return)
		{
			if (operands.Length > 0)
			{
				var a = operands[0];
				//todo: write caller-saved tags for these.
				if (a == VM.A || a == VM.B)
				{
					throw new CompilerException("Return statement would clobber non-clobbering registers.");
				}
			}
		}
	}

	private SubroutineDefinition GetFrame(string name)
	{
		return _subroutines[name];
	}

	private bool ShouldSaveRegisters()
	{
		//todo: check that this never matters in global scope. we never leave, right? e.g., shouldn't affect us linearly, only when entering/exiting calls.

		return _frames.Count > 1;
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
			frames[subroutine.Value.FrameID] = new Frame(_runner,subroutine.Value);
		}

		if (frames.Any(x => x == null))
		{
			throw new CompilerException("Multiple functions with same name?");
		}
		
		return frames;
	}
	
}