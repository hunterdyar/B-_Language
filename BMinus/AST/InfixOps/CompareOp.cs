using BMinus.AST.Enums;

namespace BMinus.AST;

public class CompareOp : BinOp
{
	public readonly Comparison Op;
	public CompareOp(Expression left, Comparison op, Expression right) : base(left, right)
	{
		this.Op = op;
	}

	public override string OpAsString => ComparisonToString(Op);

	public static string ComparisonToString(Comparison op)
	{
		switch (op)
		{
			case Comparison.Equals:
				return "==";
		}


		throw new Exception($"WHat comparison is {op}");
	}
}