using System.Linq.Expressions;
using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public class GoToParselet : IPrefixParselet
{
	public Statement Parse(Parser parser, Token token)
	{
		var destination = parser.ParseStatement();
		if (destination is Identifier id)
		{
			return new GoTo(id);
		}

		throw new ParseException("You can only GoTo identifiers");
	}
}