using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using BMinus.Parser.Parselets;
using BMinus.Tokenizer;

namespace BMinus.Parser;

public class Parser
{
	private LexerState _lexer;
	private LexerWrapper _lexWrapper;
	private List<Token> _readTokens = new List<Token>();
	private Dictionary<TokenType, IPrefixParselet> _prefixParselets = new Dictionary<TokenType, IPrefixParselet>();
	private Dictionary<TokenType, IInfixParselet> _infixParselets = new Dictionary<TokenType, IInfixParselet>();

	public Parser(Lexer lexer)
	{
		//lexer's don't keep a list of their tokens, but the wrapper does.
		//the state provides an access to the wrapper that we can easily and (more) cheaply clone for backtracking
		//i am no longer backtracking, but haven't factored it out yet.
		_lexWrapper = new LexerWrapper(lexer);
		_lexer = new LexerState(_lexWrapper);
		//prefixe dict. Thest can start statements.
		Register(TokenType.Identifier, new IdentifierParselet());
		Register(TokenType.Assignment, new AssignParselet());
		Register(TokenType.IntLiteral, new LiteralParselet());
		Register(TokenType.HexLiteral, new LiteralParselet());
		Register(TokenType.String, new LiteralParselet());
		Register(TokenType.LBrace, new CompoundStatementParselet());
		Register(TokenType.VarDeclKeyword, new VariableDeclarationParselet());
		Register(TokenType.QuestionMark, new TernaryParselet());
		Register(TokenType.LParen, new ParenthesizedExpressionParselet());
		Register(TokenType.IfKeyword, new IfStatementParselet());
		Register(TokenType.CharLiteral, new CharLiteralParselet());
		Register(TokenType.SwitchKeyword, new SwitchParselet());
		Register(TokenType.ExternKeyword, new ExternDeclarationParselet());
		Register(TokenType.ReturnKeyword, new ReturnParselet());
		//infix
		Register(TokenType.LParen, new FunctionParselet());
		
		//prefix
		Register(TokenType.Bang, new PrefixOpParselet());
		Register(TokenType.Minus, new PrefixOpParselet());
		//postfix
		
		//math
		InfixLeft(TokenType.Plus, BindingPower.Sum);
		InfixLeft(TokenType.Minus, BindingPower.Sum);
		InfixLeft(TokenType.Mod, BindingPower.Modulo);
		InfixLeft(TokenType.Asterisk, BindingPower.Product);
		InfixLeft(TokenType.Divide, BindingPower.Product);

		//conditional
		InfixLeft(TokenType.NotEquals, BindingPower.Equality);
        InfixLeft(TokenType.Equals, BindingPower.Equality);
        InfixLeft(TokenType.GreaterThan, BindingPower.NumericCompare);
        InfixLeft(TokenType.LessThan, BindingPower.NumericCompare);
        InfixLeft(TokenType.GreaterThanEqual, BindingPower.NumericCompare);
        InfixLeft(TokenType.LessThanEqual, BindingPower.NumericCompare);

        //bitwise
        InfixLeft(TokenType.ShiftLeft, BindingPower.BitwiseShift);
        InfixLeft(TokenType.ShiftRight, BindingPower.BitwiseShift);
        // InfixLeft(TokenType.BitwiseAnd, BindingPower.BitwiseAnd);
        // InfixLeft(TokenType.BitwiseOr, BindingPower.BitwiseOr);
        // InfixLeft(TokenType.BitwiseXOR, BindingPower.BitwiseXor);
		
	}

	public ProgramStatement Parse()
	{
		Statement.ResetID();
		List<Statement> rootStatements = new List<Statement>();
		bool compete = false;
		//Parse until we end up with null which we get at EOF or some surviveable error.
		do
		{
			var s = ParseStatement();
			if (s != null)
			{
				rootStatements.Add(s);
				//you don't need a ; after a }.
				if (StatementRequiresSemicolon(s))
				{
					Consume(TokenType.EndStatement);
				}
			}
			else
			{
				compete = true;
			}
		} while (!compete);

		return new ProgramStatement(rootStatements);
	}
	public Statement ParseStatement(int precedence = 0)
	{
		var token = Consume();

		//Skip over ;'s
		//We can't actually do this, will need to handle Break as a unary that has very low precedence.
		//Or maybe as a postFix that just returns the left side of the expression?
		//this is an edge-case, won't handle multiple ;;;;'s
		while (token.TokenType == TokenType.EndStatement)
		{
			token = Consume();
		}

		if (token.TokenType == TokenType.End)
		{
			return null;
		}

		if (!_prefixParselets.TryGetValue(token.TokenType, out var prefix))
		{
			throw new ParseException(
				$"Could not parse (prefix) \"{token.Literal}\" ({token.TokenType})");
		}

		Statement left = prefix.Parse(this, token);

		while (precedence < GetBindingPower())
		{
			token = Consume();

			if (!_infixParselets.TryGetValue(token.TokenType, out var infix))
			{
				throw new ParseException("Could not parse (infix) \"" + token.Literal + $"\"");
			}

			left = infix.Parse(this, left, token);
		}

		return left;
	}

	public bool Match(TokenType expected)
	{
		var token = LookAhead(0);
		if (token.TokenType != expected)
		{
			return false;
		}

		Consume();
		return true;
	}

	public Token Consume(TokenType expected)
	{
		var token = LookAhead(0);
		if (token.TokenType != expected)
		{
			if (token.TokenType == TokenType.End)
			{
				throw new ParseException($"Unexpected End-Of-Input. Expeced {expected}.");
			}
			if (expected == TokenType.EndStatement)
			{
				throw new ParseException($"Unexpected token {token.TokenType}");
			}
			else
			{
				throw new ParseException("Expected token " + expected + " and found " + token.TokenType);
			}
		}

		return Consume();
	}

	public Token Consume()
	{
		var token = LookAhead(0);
		_readTokens.Remove(token);
		return token;
	}

	public bool Peek(TokenType expected)
	{
		return LookAhead(0).TokenType == expected;
	}

	private Token LookAhead(int distance)
	{
		while (distance >= _readTokens.Count)
		{
			_readTokens.Add(_lexer.NextToken());
		}

		// Get the queued token.
		return _readTokens[distance];
	}

	private int GetBindingPower()
	{
		if (_infixParselets.TryGetValue(LookAhead(0).TokenType, out var parselet))
		{
			return parselet.GetBindingPower();
		}

		return 0;
	}

	public static bool StatementRequiresSemicolon(Statement s)
	{
		var t = s.GetType();
		return !(
			    t ==typeof(IfStatement)
		        || t == typeof(CompoundStatement)
				|| t == typeof(IfElseStatement)
			    || t == typeof(FunctionDeclaration)
				);
	}


	#region RegisterUtility

	public void Register(TokenType token, IPrefixParselet parselet)
	{
		_prefixParselets.Add(token, parselet);
	}

	public void Register(TokenType token, IInfixParselet parselet)
	{
		_infixParselets.Add(token, parselet);
	}

	/// <summary>
	/// Registers a postfix unary operator parselet for the given token and binding power.
	/// </summary>
	public void Postfix(TokenType token, int bindingPower)
	{
		Register(token, new PostfixOperatorParselet(bindingPower));
	}

	/// <summary>
	/// Registers a prefix unary operator parselet for the given token and binding power.
	/// </summary>
	public void Prefix(TokenType token, int bindingPower)
	{
		Register(token, new PrefixOperatorParselet(bindingPower));
	}

	/// <summary>
	///  Registers a left-associative binary operator parselet for the given token and binding power.
	/// </summary>
	public void InfixLeft(TokenType token, int bindingPower)
	{
		Register(token, new BinaryOperatorParselet(bindingPower, false));
	}

	/// <summary>
	/// Registers a right-associative binary operator parselet for the given token and binding power.
	/// </summary>
	public void InfixRight(TokenType token, int bindingPower)
	{
		Register(token, new BinaryOperatorParselet(bindingPower, true));
	}

	#endregion
}