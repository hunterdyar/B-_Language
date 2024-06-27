using BMinus.AST;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public interface IPrefixParselet
{
	Statement Parse(Parser parser, Token token);
}