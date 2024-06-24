using System.Data.Common;
using Ara3D.Parakeet;
using Ara3D.Parakeet.Grammars;

namespace BMinus.Barakeet;

public class BMinusGrammar : BMinusTokenGrammar
{
	public static readonly BMinusGrammar Instance = new BMinusGrammar();
	public override Rule StartRule => Program;
	
	//Generic
	public override Rule WS => Named((SpaceChars | Comment)).ZeroOrMore();
	public Rule PrefixOperator => Node(Symbols("!", "-", "+", "~"));
	public Rule PostfixOperator => Node(Symbols("++", "--") | Indexer | BinaryOperation);
	//todo: order of precedence?
	public Rule Comment => Named(CppStyleComment | CStyleBlockComment);
	// public Rule FunctionArgs => Node(ParenthesizedList(Expression));

	//Fragments
	public Rule Indexer => Node(Bracketed(Expression));
	//Statements
	public Rule Statement => Node((Assignment | VariableDeclaration) + AdvanceOnFail);
	public Rule VariableDeclaration => Node(DeclarationKeyword + ListOfAtLeastOne(Identifier, Comma.Optional()));
	public Rule Assignment => Node(Identifier + AssignmentChar + Expression);
	
	//Expressions

	public Rule LeafExpression => 
		Literal 
		| ParenthesizedExpression
		| Identifier 
	;

	public Rule PrefixOperation => PrefixOperator + LeafExpression;
	public Rule PostfixOperation => Expression + PostfixOperator;
	public Rule ParenthesizedExpression => Parenthesized(Expression);
	public Rule BinaryOperation => Node(BinaryOperator + AdvanceOnFail+ Expression);
	public Rule InnerExpression => PrefixOperation | PostfixOperation | LeafExpression;
	public Rule Expression => Node(Recursive(nameof(InnerExpression)));
	public Rule Block => BracedList(Statement,Break);
	
	//Program
	public Rule Program => Node(ListOfAtLeastOne(WS + Statement+AdvanceOnFail,Break)+EndOfInput);
	
}