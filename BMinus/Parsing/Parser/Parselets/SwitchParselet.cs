using BMinus.AST;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public class SwitchParselet : IPrefixParselet
{
	public Statement Parse(Parser parser, Token token)
	{
		throw new NotImplementedException("Switch statements not yet implemented");
	}
}