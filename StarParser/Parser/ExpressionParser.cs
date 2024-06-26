using BMinus.AST;
using StarParser.Tokenizer;

namespace StarParser.Parser;

public partial class Parser
{
	public static bool TryExpression(LexerState lex, out Expression s)
	{
		var a = lex.Clone();
		if (TryLiteral(a, out s))
		{
			return true;
		}

		return false;
	}
	public static bool TryLiteral(LexerState lex, out Expression expression)
	{
		var t = lex.NextToken();
		if (t.TokenType == TokenType.IntLiteral)
		{
			expression = new WordLiteral(int.Parse(t.Literal));
			return true;
		}
		else if (t.TokenType == TokenType.HexLiteral)
		{
			//todo
			expression = new WordLiteral(0);
			return true;
		}
		else if (t.TokenType == TokenType.String)
		{
			//todo
			expression = new WordLiteral(0); //array
			return true;
		}

		expression = null;
		return false;
	}
}