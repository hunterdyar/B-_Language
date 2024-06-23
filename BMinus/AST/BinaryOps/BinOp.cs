using BMinus.Parser;
using Superpower.Model;

namespace BMinus.AST;

public abstract class BinOp : Expression
{
	public readonly Expression Left;
	public readonly Expression Right;
	public virtual string OpAsString => "UDEFINED";
	protected BinOp(Expression left, Expression right)
	{
		Left = left;
		Right = right;
	}
	public override string ToString()
	{
		return $"({Left.ToString()} {OpAsString} {Right.ToString()} )";
	}

	public static BinOp GetBinaryOp(Expression left, Token<BMToken> token, Expression right)
	{
		switch (token.Kind)
		{
			case BMToken.Plus:
				return new AddExpr(left, right);
			case BMToken.Minus:
				return new SubtractExpr(left, right);
			default:
				throw new ArgumentException("bad token");
		}
	}
}