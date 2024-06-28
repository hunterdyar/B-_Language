using BMinus.AST;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public class CharLiteralParselet : IPrefixParselet
{
	public Statement Parse(Parser parser, Token token)
	{
		return new WordLiteral('c',token.Literal);
	}
}