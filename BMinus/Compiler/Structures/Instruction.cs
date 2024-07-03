using System.Runtime.InteropServices.JavaScript;

namespace BMinus.Compiler;

[System.Serializable]
public struct Instruction 
{
	public OpCode Op;
	public int OperandA;
	public int OperandB;
	public uint ASTNodeID;
	public Instruction(OpCode op, uint astNodeId)
	{
		this.Op = op;
		OperandA = 0;
		OperandB = 0;
		ASTNodeID = astNodeId;
	}

	public Instruction(OpCode op, uint astNodeId, int operand)
	{
		this.Op = op;
		OperandA = operand;
		OperandB = 0;
		ASTNodeID = astNodeId;
		ASTNodeID = astNodeId;
	}

	public Instruction(OpCode op, uint astNodeId, int operandA, int operandB)
	{
		this.Op = op;
		OperandA = operandA;
		OperandB = operandB;
		ASTNodeID = astNodeId;
	}

	public Instruction(OpCode op, uint astNodeId = 0, params int[] operands)
	{
		this.Op = op;
		if (operands.Length > 0)
		{
			OperandA = operands[0];
		}

		if (operands.Length > 1)
		{
			OperandB = operands[1];
		}

		ASTNodeID = astNodeId;
	}

	public void SetA(int a)
	{
		OperandA = a;
	}

	public void SetB(int b)
	{
		OperandA = b;
	}

	public override string ToString()
	{
		return $"{Op} - {OperandA} | {OperandB}";
	}
	
}