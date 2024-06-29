﻿using System.Runtime.Serialization;
using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using BMinus.Models;
using VM = BMinus.VirtualMachine.VirtualMachine;
namespace BMinus.Compiler;

public class Compiler
{
	public readonly Statement Root;
	private int _currentFrame;

	//shorthands for register indices. can move to VM as static.

	//environment
	//functions, basically
	private SubroutineDefinition Frame => _frames[_currentFrame];
	private List<SubroutineDefinition> _frames = new List<SubroutineDefinition>();

	public Compiler(Statement s)
	{
		Root = s;
		_frames.Add(new SubroutineDefinition(0));//global frame.
		_currentFrame = 0;
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
			var left = assignment.Identifier;
			int leftID = 0;//todo
			CompileExpression(assignment.ValueExpr, VM.EAX);
			//if isLocal
			Emit(OpCode.SetLocal, leftID, VM.EAX);
			//else
			//Emit(OpCode.SetGlobal, leftID, EAX);

		}else if (statement is VariableDeclaration varDeclaration)
		{
			foreach (var identifier in varDeclaration.Identifiers)
			{
				Frame.AddLocal(identifier.Value);
			}
		} else if(statement is ExternDeclaration externDeclaration)
		{
			foreach (var var in externDeclaration.Identifiers)
			{
				//add identifier to our variable resolution system.
			}
		}else if (statement is FunctionCall fn)
		{
			//compile and put onto the stack in order. 
			foreach (var arg in fn.Arguments)
			{
				CompileExpression(arg,-1);
			}

			//getFunctionID = fn.FunctionName;
			
			int fnVal = 0;
			Emit(OpCode.Call, fnVal);
			//entering a frame sets base stackPointer, etc.
			//after the function is done, we will return to this location of this frame.
			//Runtime frames will clean the stack when we leave them.
		}else if (statement is FunctionDeclaration fnDec)
		{
			//set a function prototype frame ID for name
			//create a new frame
			//in this frame, compile the arguments into gets from the stack (eh?) into local variables (id 0,1,2,etc)
			//we don't need to use globals, we can use local frame environment...
			Emit(OpCode.Return);//Leaves the frame, (which cleans the stack from it's locals). With a value perhaps in EAX.
		}else if(statement is GoTo gotoStatement)
		{
			//todo: a temporary instruction type would be fine. THen we finish compiling, and we search for the label, etc.
			//dynamic goto's are not supported, this is compile time.
			// string label = temporary LabelValue.
			//destination = getdestination
			//if we are not in the current frame, we need to clean the stack?
			//shit.
			Emit(OpCode.GoTo, 0, 0);//
		}else if (statement is IfStatement ifStatement)
		{
			
		}else if (statement is Label label)
		{
			//Create label
			
		}else if (statement is Nop nop)
		{
			Emit(OpCode.Nop);
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
			Emit(OpCode.SetReg,wordLiteral.ValueAsInt, register);
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
			Emit(OpCode.Arithmetic, (int)binMathOp.Op, register);//leaves result in EAX
		}else if (expression is CompareOp compareOp)
		{
			CompileExpression(compareOp.Left, VM.A);
			CompileExpression(compareOp.Right, VM.B);
			Emit(OpCode.Compare, (int)compareOp.Op, register);
		}else if (expression is TernaryOp ternary)
		{
			//ternary's are if's, except they leave a value in the register.
			CompileExpression(ternary.Condition, VM.EAX);
			var j_nq = Emit(OpCode.JumpNotEq, 0);
			CompileExpression(ternary.Consequence, VM.A);
			var la = TopLocation();
			var j_skipAlt = Emit(OpCode.Jump);
			CompileExpression(ternary.Alternative, VM.B);
			var lb = TopLocation();
			UpdateOperands(j_nq, j_skipAlt.FrameIndex, j_skipAlt.InstructionIndex);//+1?
			UpdateOperands(j_skipAlt, lb.FrameIndex, lb.InstructionIndex);

			//get position?
		}else if (expression is Identifier identifier)
		{
			if (Frame.TryGetLocal(identifier.Value, out int index))
			{
				Emit(OpCode.GetLocal, index, register);
				return;
			}
			//todo: globals.
			//get heap value, and put into register.
		}
	}
	
	#region Helpers
	private InstructionLocation Emit(OpCode code, params int[] operands)
	{
		if (operands.Length == 0)
		{
			return Frame.AddInstruction(new Instruction(code));
		}else if (operands.Length == 1)
		{
			return Frame.AddInstruction(new Instruction(code, operands[0]));
		}
		else if (operands.Length == 2)
		{
			return Frame.AddInstruction(new Instruction(code, operands[0], operands[1]));
		}else
		{
			throw new CompilerException("Too many operands");
		}
	}

	private SubroutineDefinition GetFrame(int index)
	{
		return _frames[index];
	}
	
	private InstructionLocation TopLocation()
	{
		return Frame.GetTopInstructionLocation();
	}

	private void UpdateOperands(InstructionLocation original, params int[] newOps)
	{
		var f = GetFrame(original.FrameIndex);
		f.UpdateOperands(original,newOps);
	}
	
	#endregion

	public Environment.Environment GetEnvironment()
	{
		return new Environment.Environment(GetFrames());
	}

	public Frame[] GetFrames()
	{
		var frames = new Frame[_frames.Count];
		for (int i = 0; i < _frames.Count; i++)
		{
			frames[i] = new Frame(_frames[i]);
		}

		return frames;
	}
}