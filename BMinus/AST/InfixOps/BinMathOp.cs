using BMinus.Models;

namespace BMinus.AST;

public class BinMathOp : BinOp
{
	public readonly BinaryArithOp Op;
	public override string OpAsString => ComparisonToString(Op);
	public BinMathOp(Expression left, BinaryArithOp op, Expression right) : base(left, right)
	{
		this.Op = op;
	}

	public static string ComparisonToString(BinaryArithOp op)
	{
		switch (op)
		{
			case BinaryArithOp.Add:
				return "+";
			case BinaryArithOp.Subtract:
				return "-";
			case BinaryArithOp.Multiply:
				return "*";
			case BinaryArithOp.Divide:
				return "/";
			case BinaryArithOp.Remainder:
				return "%";
		}
		throw new Exception($"Huh? comparison is {op}");
	}
}