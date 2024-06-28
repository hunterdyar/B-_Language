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
	public short EAX => 0;//accumulator
	public short A => 1;//general
	public short B => 2;//general
	public short C => 3;//count
	public short D => 4;
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
	/// <param name="expression">Expression to generate instruction to put into environment</param>
	/// <param name="register">which register</param>
	/// <param name="toRegister">true for registers, false for stack</param>
	public void CompileExpression(Expression expression, int register = 0, bool toRegister = true)
	{
		if(expression is WordLiteral wordLiteral)
		{
			
			//add to register? 
			//add instruction, push wordLiteral.GetValue.
		}else if (expression is BinMathOp binMathOp)
		{
			CompileExpression(binMathOp.Left, A);
			CompileExpression(binMathOp.Right, B);
			Emit(OpCode.Arithmetic, (short)binMathOp.Op);//leaves result in EAX
			if (toRegister)
			{
				if (register != 0)//0 is eax
				{
					Emit(OpCode.MoveReg, 0, 1);//move data from register op0 to register op1, 
				}
				
			}
		}
	}

	#region Helpers

	private void Emit(OpCode code, params short[] operands)
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