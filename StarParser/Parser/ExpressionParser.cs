using BMinus.AST;
using StarParser.Tokenizer;

namespace StarParser.Parser;

public partial class Parser
{
	public static bool TryExpression(LexerState lex, out Expression s)
	{
		var a = lex.Clone();
		if (TryInfixOperation(a, out s))
		{
			lex.From(a);
			return true;
		}
		
		a = lex.Clone();
		if (TryLiteral(a, out s))
		{
			lex.From(a);
			return true;
		}

		return false;
	}

	public static LexerState? InfixState; 
	public static bool TryInfixOperation(LexerState lex, out Expression s)
	{
		//Brute recursion block. We can't explore this node if we're already exploring it.
		if (InfixState != null)
		{
			if (InfixState.SameAs(lex))
			{
				s = null;
				return false;
			}
		}

		InfixState = lex;

		if (TryExpression(lex, out Expression left))
		{
			var op = lex.NextToken();
			if (Token.IsInfixOp(op.TokenType))
			{
				if (TryExpression(lex, out Expression right))
				{
					s = BinOp.GetBinaryOp(left, op.Literal, right);
					return true;
				}
			}
		}

		s = null;
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