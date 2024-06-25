using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
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
	},(x)=>new ProgramStatement(x));

	public static ParseNode Identifier => new ParseNodeLeaf
	{
		CanCreateNode = (t) => t.TokenType == TokenType.Identifier
	};

	public static ParseNode MatchToken(TokenType t) => new ParseNodeLeaf()
	{
		CanCreateNode = (x) => x.TokenType == t,
	};

	public static ParseNode Parse(Lexer lexer)
	{
		//making this static is ugly.
		ParseNode.Lexer = lexer;
		StartNode.Parse(0, out var c);
		//if c != length...
		return StartNode;
	}
}