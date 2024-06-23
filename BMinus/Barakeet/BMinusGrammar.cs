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
	public Rule PostfixOperator => Node(Symbols("++", "--") | Indexer | FunctionArgs);
	//todo: order of precedence?
	public Rule Comment => Named(CppStyleComment | CStyleBlockComment);
	public Rule FunctionArgs => Node(ParenthesizedList(Expression));


	//Fragments
	public Rule Indexer => Node(Bracketed(Expression));
	//Statements
	public Rule Statement => Node((Assignment | VariableDeclaration) + AdvanceOnFail);
	public Rule VariableDeclaration => Node(DeclarationKeyword + ListOfAtLeastOne(Identifier, Comma.Optional()));
	public Rule Assignment => Node(Identifier + AssignmentChar + Expression);
	
	//Expressions

	public Rule LeafExpression => 
		Literal 
		//parenthesized expression
		| Identifier 
	;
	public Rule BinaryExpression => Node(Expression + BinaryOperator + Expression);
	public Rule InnerExpression => Node(PrefixOperator.ZeroOrMore() + LeafExpression + PostfixOperator.ZeroOrMore());
	public Rule OuterExpression => Node(InnerExpression);
	public Rule Expression => Node(Recursive(nameof(OuterExpression)));
	public Rule Block => BracedList(Statement,Break);
	
	//Program
	public Rule Program => Node(ListOfAtLeastOne(WS + Statement+AdvanceOnFail,Break)+EndOfInput);
	
}