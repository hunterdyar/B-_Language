using BMinus.AST;
using StarParser.Tokenizer;

namespace StarParser.Parser;

public abstract class ParseNode
{
	public static Lexer Lexer;
	public Token Get(int i) => Lexer.GetToken(i);
	public bool IsValid;

	public virtual Func<Statement> GetASTNode { get; init; }
	//It should take the entire token state! Not called in from above. That way we don't need a "isValid" check
	public abstract bool Parse(int nextToken, out int consumed);
}

public class ParseNodeLeaf : ParseNode
{
	public Func<Token, bool> CanCreateNode;
	public Func<Token, Statement> CreateASTNode;
	public override Func<Statement> GetASTNode => ()=>_statement;
	private Statement _statement;
	private Token token;
	public override bool Parse(int token, out int consumed)
	{
		this.token = Get(token);
		IsValid = CanCreateNode(Get(token));
		if (IsValid)
		{
			_statement = CreateASTNode(Get(token));
		}

		consumed = 1;
		return IsValid;
	}

	public ParseNodeLeaf()
	{
		
	}
}

public class ParseNodeChoice : ParseNodeRule
{
	private ParseNode firstValidNode;
	public override Func<Statement> GetASTNode => firstValidNode.GetASTNode;

	public override bool Parse(int token, out int consumed)
	{
		IsValid = false;
		consumed = 0;
		foreach (var c in Children)
		{
			if (c.Parse(token,out var eat))
			{
				IsValid = true;
				firstValidNode = c;
				consumed += eat;
				return true;
			}
		}

		return false;
	}

	public ParseNodeChoice(ParseNode[] children) : base(children)
	{
	}
}

public class ParseNodeSequence : ParseNodeRule
{
	public Func<List<Statement>, Statement> CreateASTNode;
	private List<Statement> _sequence = new List<Statement>();
	public override Func<Statement> GetASTNode => ()=>CreateASTNode(_sequence);

	public override bool Parse(int token, out int consumed)
	{
		IsValid = true;
		_sequence = new List<Statement>();
		int t = token;
		consumed = 0;
		foreach (var c in Children)
		{
			if (!c.Parse(t, out int eat))
			{
				IsValid = false;
				consumed += eat;
				return false;
			}
			else
			{
				_sequence.Add(c.GetASTNode());
			}
			t++;
		}

		return true;
	}

	public ParseNodeSequence(ParseNode[] children, Func<List<Statement>,Statement> getAst) : base(children)
	{
		this.CreateASTNode = getAst;
	}
}

public class ZeroOrMore : ParseNodeRule
{
	public List<ParseNode> Children => _children;
	private List<ParseNode> _children = new List<ParseNode>();
	public Func<ParseNode> GetParseNode;
	public ZeroOrMore(ParseNode[] children) : base(children)
	{
		IsValid = true;
	}

	public override bool Parse(int token, out int consumed)
	{
		//clone
		consumed = 0;
		bool failed = false;
		while(!failed){
			var n = GetParseNode();
			if (n.Parse(token, out int eat))
			{
				_children.Add(n);
				consumed += eat;
			}
			else
			{
				failed = true;
				break;
			}
		}
		//todo: how many tokens were consumed?

		return true;
	}
}
public abstract class ParseNodeRule : ParseNode
{
	public ParseNode[] Children;

	protected ParseNodeRule(ParseNode[] children)
	{
		Children = children;
	}
}