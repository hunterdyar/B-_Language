using BMinus.AST;
using BMinus.Tokenizer;
using BMinus;

namespace BMinus.Parser.Parselets;

public interface IInfixParselet
{
	Statement Parse(Parser parser, Statement left, Token token);
	int GetBindingPower();
}