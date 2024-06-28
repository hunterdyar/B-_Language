using System.Runtime.Serialization;
using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using BMinus.Models;

namespace BMinus.Compiler;

public class Compiler
{
	public readonly Statement Root;
	private Frame Frame => _frames[_currentFrame];
	private int _currentFrame;

	//shorthands for register indices.
	public int EAX => 0;//accumulator
	public int A => 1;//general
	public int B => 2;//general
	public int C => 3;//count
	public int D => 4;
	//environment
	//frames
	private List<Frame> _frames = new List<Frame>();

	public Compiler(Statement s)
	{
		Root = s;
		_frames.Add(new Frame(0));//global frame.
		_currentFrame = 0;
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
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="expression">Generate a value that that gets put into a register or onto the stack.</param>
	/// <param name="register">which register, or -1 for on the stack.</param>
	public void CompileExpression(Expression expression, int register = 0)
	{
		if(expression is WordLiteral wordLiteral)
		{
			//add to register? 
			//add instruction, push wordLiteral.GetValue.
		}else if (expression is BinMathOp binMathOp)
		{
			CompileExpression(binMathOp.Left, A);
			CompileExpression(binMathOp.Right, B);
			Emit(OpCode.Arithmetic, (short)binMathOp.Op, register);//leaves result in EAX
		}
	}

	#region Helpers

	private void Emit(OpCode code, params int[] operands)
	{
		if (operands.Length == 0)
		{
			Frame.AddInstruction(new Instruction(code));
		}else if (operands.Length == 1)
		{
			Frame.AddInstruction(new Instruction(code, operands[0]));
		}
		else if (operands.Length == 1)
		{
			Frame.AddInstruction(new Instruction(code, operands[0], operands[1]));
		}else
		{
			throw new CompilerException("Too many operands");
		}
	}
	

	#endregion
}