using Superpower.Display;
using Superpower.Parsers;

namespace BMinus.Parser;

public enum BMToken
{
	None,
	[Token(Category = "operator", Example = "+")]
	Plus,
	[Token(Category = "operator", Example = "-")]
	Minus,
	[Token(Category = "operator", Example = "*")]
	Times,
	[Token(Category = "operator", Example = "/")]
	Divide,
	[Token(Example = "(")]
	LParen,
	[Token(Example = ")")]
	RParen,
	[Token(Example = "{")]
	LBrace,
	[Token(Example = "}")]
	RBrace,
	[Token(Example = "[")]
	LSquareBracket,
	[Token(Example = "]")]
	RSquareBracket,
	[Token(Category = "identifier")]
	Identifier,
	[Token(Category = "keyword", Example = "var")]
	VarKeyword,
	[Token(Example = ";")]
	EndExpression,
	[Token(Category = "literal")]
	String,
	[Token(Category = "literal")]
	IntLiteral,
	[Token(Category = "literal")]
	HexLiteral,
	[Token(Category = "operator", Example = "=")]
	Assign,
}