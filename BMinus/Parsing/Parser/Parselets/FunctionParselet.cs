using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using BMinus.Tokenizer;

namespace BMinus.Parser.Parselets;

public class FunctionParselet : IInfixParselet
{
	public Statement Parse(Parser parser, Statement left, Token token)
	{
		if (left is not Identifier id)
		{
			throw new ParseException("Function Declaration must start with an identifier - (invalid function name)");
		}
		
		//lparen consumed by infix
		
		//arguments
		var args = new List<Statement>();
		if (!parser.Peek(TokenType.RParen))
		{
			do
			{
				var s = parser.ParseStatement();
				args.Add(s);
			} while (parser.Match(TokenType.Comma));
		}

		parser.Consume(TokenType.RParen);
		if (parser.Peek(TokenType.LBrace))
		{
			//this should be a statement block?
			var block = parser.ParseStatement();
			var parameters = args.Select((x => x as Identifier)).Where(x=>x!=null).ToList();
			if (parameters.Count != args.Count)
			{
				//todo: rewrite this for better errors.
				throw new ParseException("Bad function definition. Only identifiers can be used as parameters.");
			}
			return new FunctionDeclaration(id, parameters, block); // func identity(){}

		}else if (parser.Peek(TokenType.EndStatement))
		{
			var arguments = args.Select((x => x as Expression)).Where(x => x != null).ToList();
			if (arguments.Count != args.Count)
			{
				//todo: rewrite this for better errors.
				throw new ParseException("Bad function call. Only identifiers can be used.");
			}
			return new FunctionCall(id,arguments);
		}

		

		//consume function signature
		//consume expression block
		throw new ParseException("Funtion parse error");
	}

	public int GetBindingPower()
	{
		//todo: check this?
		return BindingPower.Call;
	}
}