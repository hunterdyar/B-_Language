using BMinus.Tokenizer;

namespace BMinus.Models;

public enum Comparison
{
	Equals = TokenType.Equals,
	NotEquals = TokenType.NotEquals,
	LessThan = TokenType.LessThan,
	LessThanOrEqual = TokenType.LessThanEqual,
	GreaterThan = TokenType.GreaterThan,
	GreaterThanOrEqual = TokenType.GreaterThanEqual
}