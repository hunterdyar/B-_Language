using BMinus.Models;

namespace BMinus.AST;

public class BinMathOp : BinOp
{
	public readonly BinaryArith Op;
	public override string OpAsString => ComparisonToString(Op);
	public BinMathOp(Expression left, BinaryArith op, Expression right) : base(left, right)
	{
		this.Op = op;
	}

	public static string ComparisonToString(BinaryArith op)
	{
		switch (op)
		{
			case BinaryArith.Add:
				return "+";
			case BinaryArith.Subtract:
				return "-";
			case BinaryArith.Multiply:
				return "*";
			case BinaryArith.Divide:
				return "/";
			case BinaryArith.Remainder:
				return "%";
		}
		throw new Exception($"Huh? comparison is {op}");
	}
}