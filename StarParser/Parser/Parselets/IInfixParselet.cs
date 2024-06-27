using BMinus.AST;
using StarParser;
using StarParser.Tokenizer;

namespace StarParser.Parser.Parselets;

public interface IInfixParselet
{
	Statement Parse(Parser parser, Statement left, Token token);
	int GetBindingPower();
}