using BMinus.AST;
using StarParser.Tokenizer;

namespace StarParser.Parser;

public class Parser
{
	private ParseNode Root = StartNode;

	public static ParseNode StartNode = new ParseNodeSequence(
	new []{
		MatchToken(TokenType.LParen),
		Identifier,
		MatchToken(TokenType.RParen)
	});

	public static ParseNode Identifier => new ParseNodeLeaf()
	{
		CreateASTNode = (t) => new Identifier(t.Literal),
		CanCreateNode = (t) => t.TokenType == TokenType.Identifier
	};

	public static ParseNode MatchToken(TokenType i) => new ParseNodeLeaf()
	{
		CanCreateNode = (t) => t.TokenType == i
	};

	public static ParseNode Parse(Lexer lexer)
	{ 
		StartNode.Parse(0, out var c);
		//if c != length...
		return StartNode;
	}
}