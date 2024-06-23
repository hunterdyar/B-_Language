using BMinus.Parser;
using Superpower.Model;

namespace BMinus.AST;

public class Identifier : Expression
{
	public readonly string Value;
	public Identifier(Token<BMToken> token)
	{
		Value = token.Span.ToString();
	}

	public override string ToString()
	{
		return Value;
	}
}