using BMinus.AST;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public class LabelParselet : IPrefixParselet
{
	public Statement Parse(Parser parser, Token token)
	{
		return new Label(token.Literal);
	}
}