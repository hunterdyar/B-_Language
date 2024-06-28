namespace BMinus.Compiler;

public struct Instruction
{
	public OpCode Op;
	public short OperandA;
	public short OperandB;

	public Instruction(OpCode op)
	{
		this.Op = op;
		OperandA = 0;
		OperandB = 0;
	}

	public Instruction(OpCode op, short operand)
	{
		this.Op = op;
		OperandA = operand;
		OperandB = 0;
	}

	public Instruction(OpCode op, short operandA, short operandB)
	{
		this.Op = op;
		OperandA = operandA;
		OperandB = operandB;
	}
}