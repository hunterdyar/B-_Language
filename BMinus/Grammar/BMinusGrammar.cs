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

	public Rule PostfixOperator =>
		Symbols("++", "--") 
		     | Indexer 
		     | InfixOperation
		     | TernaryOperation
		     | BinaryOperation;
	//todo: order of precedence?
	public Rule Comment => Named(CppStyleComment | CStyleBlockComment);
	// public Rule FunctionArgs => Node(ParenthesizedList(Expression));

	//Fragments
	public Rule Indexer => Node(Bracketed(Expression));
	
	//Statements
	public Rule Statement => Recursive(nameof(InnerStatement));
	public Rule VariableDeclaration => Node(DeclarationKeyword + ListOfAtLeastOne(Identifier, Comma.Optional()));
	public Rule Assignment => Node(Identifier + AssignmentChar + Expression);
	public Rule ExpressionStatement => Node(Expression + RecoverEos + Break);
	public Rule Block => BracedList(Statement, Break);

	public Rule InnerStatement => Named(
		Break
		| VariableDeclaration
		| Assignment
		//| ExpressionStatement//must have lowest precedence
		);
	
	//Expressions
	public Rule LeafExpression => 
		Literal
		| ParenthesizedExpression
		| Identifier 
	;

	public Rule TernaryOperation => Node(Sym("?") + Expression + Sym(":") + Expression);
	public Rule BinaryOperation => Node(Not("=>") + BinaryOperator + RecoverEos + Expression);
	//public Rule PostfixOperation => Node(Node(Recursive(nameof(Expression))) + PostfixOperator);
	public Rule ParenthesizedExpression => Node(Parenthesized(Expression));
	//infix is defined as a postfix. that is a++, ++ is postfix. a+b, +b is the postfix. together they get turned into infix when going from untyped to typed ast (syntaxtreebuilder)
	public Rule InfixOperation => Node(Not("{")+BinaryOperator + Expression);
	public Rule InnerExpression => Node(PrefixOperator.ZeroOrMore() + LeafExpression + PostfixOperator.ZeroOrMore());
	public Rule Expression => Node(Recursive(nameof(InnerExpression)));
	
	//Program
	public Rule Program => Node(ListOfAtLeastOne(WS + Statement+AdvanceOnFail,Break)+EndOfInput);
	
}