
using BMinus.Models;

namespace BMinus.AST;

public abstract class BinOp : Expression
{
	public readonly Expression Left;
	public readonly Expression Right;
	public virtual string OpAsString => "UDEFINED";
	protected BinOp(Expression left, Expression right)
	{
		Left = left;
		Left.Label = "Left";
		Right = right;
		Right.Label = "Right";
	}
	public override string ToString()
	{
		return $"({Left.ToString()} {OpAsString} {Right.ToString()})";
	}

	protected override IEnumerable<Statement> GetChildren()
	{
		return new[] { Left, Right };
	}

	protected override string GetJSONName()
	{
		return "Op ("+OpAsString+")";
	}

	public static BinOp GetBinaryOp(Expression left, string op, Expression right)
	{
		switch (op)
		{
			case "+":
				return new BinMathOp(left,BinaryArithOp.Add, right);
			case "-":
				return new BinMathOp(left, BinaryArithOp.Subtract, right);
			case "*":
				return new BinMathOp(left, BinaryArithOp.Multiply, right);
			case "/":
				return new BinMathOp(left, BinaryArithOp.Divide, right);
			case "%":
				return new BinMathOp(left, BinaryArithOp.Remainder, right);
			case "==":
				return new CompareOp(left, Comparison.Equals, right);
			case ">":
				return new CompareOp(left, Comparison.GreaterThan, right);
			case ">=":
				return new CompareOp(left, Comparison.GreaterThanOrEqual, right);
			case "<":
				return new CompareOp(left, Comparison.LessThan, right);
			case "<=":
				return new CompareOp(left, Comparison.LessThanOrEqual, right);
			case "!=":
				return new CompareOp(left, Comparison.NotEquals, right);
			default:
				throw new ArgumentException($"bad token for binary op: {op}");
		}
	}
	
}