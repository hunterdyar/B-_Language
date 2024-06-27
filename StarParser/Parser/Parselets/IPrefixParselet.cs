using BMinus.AST;
using StarParser.Tokenizer;

namespace StarParser.Parser.Parselets;

public interface IPrefixParselet
{
	Statement Parse(Parser parser, Token token);
}