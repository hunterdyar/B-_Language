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
}