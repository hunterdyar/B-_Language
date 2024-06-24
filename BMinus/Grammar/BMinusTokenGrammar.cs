using Ara3D.Parakeet;
using Ara3D.Parakeet.Grammars;

namespace BMinus.Barakeet;

public class BMinusTokenGrammar : BaseCommonGrammar
{
	//Keywords and Symbols
	public Rule Break => Sym(";");
	public Rule DeclarationKeyword => Keyword("var");
	public Rule AssignmentChar => Sym("=");
	public Rule AdvanceOnFail => new OnFail(AdvanceToEnd);
	public Rule EscapedLiteralChar => Named('\\' + AnyChar);
	public Rule StringLiteralChar => Named(EscapedLiteralChar | "\"\"" | AnyChar.Except('"'));
	public Rule CharLiteralChar => Named(EscapedLiteralChar | AnyChar.Except('\''));
	public Rule IntegerSuffix => Named(Strings("ul", "UL", "u", "U", "l", "L", "lu", "lU", "Lu", "LU"));
	public Rule FloatSuffix => Named("fFdDmM".ToCharSetRule());
	public Rule IntegerLiteral => Node(Digits.ThenNot("fFdDmM".ToCharSetRule()) + IntegerSuffix.Optional());
	public Rule FloatLiteral => Node(Float + FloatSuffix.Optional());
	public Rule BinaryLiteral => Node("0b" | "0B" + BinDigit.OneOrMore() + IntegerSuffix.Optional());
	public Rule HexLiteral => Node(Strings("0x", "0X") + HexDigit.OneOrMore() + IntegerSuffix.Optional());
	public Rule StringLiteral => Node(Optional('@') + '"' + StringLiteralChar.ZeroOrMore() + '"');
	public Rule CharLiteral => Node('\'' + CharLiteralChar + '\'');
	public Rule BooleanLiteral => Node(Keyword("true") | Keyword("false"));

	public Rule Literal => 
		HexLiteral
		| BinaryLiteral
		| FloatLiteral
		| IntegerLiteral
		| StringLiteral
		| CharLiteral
		| BooleanLiteral
		;

	public Rule BinaryOperator => Node(Symbols(
		"+", "-", "*", "/", "%", ">>>", ">>", "<<", "&&", "||", "&", "|", "^",
		"+=", "-=", "*=", "/=", "%=", ">>>=", ">>=", "<<=", "&&=", "||=", "&=", "|=", "^=",
		"=", "<", ">", "<=", ">=", "==", "!=",
		"??", "?="
	));

	public Rule RecoverEos => OnFail(
		CharLiteralChar.Except(Break).ZeroOrMore()
		+ (Break | EndOfInput) | AdvanceToEnd);
}
