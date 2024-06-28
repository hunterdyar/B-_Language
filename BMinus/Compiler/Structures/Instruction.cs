namespace BMinus.Compiler;

public struct Instruction
{
	public OpCode Op;
	public int OperandA;
	public int OperandB;

	public Instruction(OpCode op)
	{
		this.Op = op;
		OperandA = 0;
		OperandB = 0;
	}

	public Instruction(OpCode op, int operand)
	{
		this.Op = op;
		OperandA = operand;
		OperandB = 0;
	}

	public Instruction(OpCode op, int operandA, int operandB)
	{
		this.Op = op;
		OperandA = operandA;
		OperandB = operandB;
	}

	public Instruction(OpCode op, params int[] operands)
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
	}

	public void SetA(int a)
	{
		OperandA = a;
	}

	public void SetB(int b)
	{
		OperandA = b;
	}
}