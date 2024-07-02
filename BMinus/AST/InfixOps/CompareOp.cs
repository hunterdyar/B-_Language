using BMinus.Models;

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
			case Comparison.GreaterThan:
				return ">";
			case Comparison.LessThan:
				return "<";
			case Comparison.GreaterThanOrEqual:
				return ">=";
			case Comparison.LessThanOrEqual:
				return "<=";
			case Comparison.NotEquals:
				return "!=";
		}
		
		throw new Exception($"Huh? comparison is {op}");
	}
}