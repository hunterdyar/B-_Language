﻿namespace StarParser.Tokenizer;
public enum TokenType
{
	Identifier,
	IntLiteral,
	HexLiteral,
	LParen,
	RParen,
	LBrace,
	RBrace,
	LBracket,
	RBracket,
	Comma,
	Colon,
	QuestionMark,
	EndStatement,
	VarDeclKeyword,
	ExternKeyword,
	IfKeyword,
	WhileKeyword,
	SwitchKeyword,
	Bang,
	LessThan,
	Assignment,
	NotEquals,
	GreaterThanEqual,
	LessThanEqual,
	Equals,
	GreaterThan,
	Tilde,
	Asterisk,
	Plus,
	Increment,
	Minus,
	Divide,
	Decrement,
	AssignmentPlus,
	AssignmentMinus,
	ShiftLeft,
	ShiftRight,
	And,
	Mod,
	GoToKeyword,
	CaseKeyword,
	DefaultKeyword,
	BreakKeyword,
	ReturnKeyword,
	Address,
	Indirection,
	String,
	End,
}