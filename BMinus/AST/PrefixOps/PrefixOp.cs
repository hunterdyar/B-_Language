using BMinus.Models;
using BMinus.Parser;

namespace BMinus.AST;

public class PrefixOp : Expression
{
	public readonly Expression Right;
	public readonly UnaryPrefixOp Op;
	public PrefixOp(Expression right, UnaryPrefixOp op)
	{
		this.Right = right;
		Op = op;
	}
	public static PrefixOp GetPrefixOp(Expression right, string op)
	{
		switch (op)
		{
			case "-":
				//check if right is literal (optimize!)
				return new PrefixOp(right, UnaryPrefixOp.Negate);
			case "!":
				return new PrefixOp(right, UnaryPrefixOp.Not);
		}
		
		throw new ArgumentException($"Unable to create prefix expression with op: '{op}'");
	}

	protected override IEnumerable<Statement> GetChildren()
	{
		return new[] { Right };
	}

	public override string ToString()
	{
		return UnaryPrefixOpToString(this.Op) + Right.ToString();
	}

	public static string UnaryPrefixOpToString(UnaryPrefixOp op)
	{
		switch (op)
		{
			case UnaryPrefixOp.Negate:
				return "-";
			case UnaryPrefixOp.Not:
				return "!";
		}

		throw new ParseException($"Bad unary prefix op {op}, shouldn't be trying to stringify it here.");
	}
}