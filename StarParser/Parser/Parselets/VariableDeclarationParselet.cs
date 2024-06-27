﻿using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using StarParser.Tokenizer;

namespace StarParser.Parser.Parselets;

public class VariableDeclarationParselet : IPrefixParselet
{
	public Statement Parse(Parser parser, Token token)
	{
		var variables = new List<Identifier>();
		if (!parser.Peek(TokenType.EndStatement))
		{
			do
			{
				var idTok = parser.Consume(TokenType.Identifier);
				variables.Add(new Identifier(idTok.Literal));
			} while (parser.Match(TokenType.Comma));
		}

		if (variables.Count == 0)
		{
			throw new ParseException(
				$"Empty variable declaration. Must declare at least one variable after {token.Literal}");
		}
		else
		{
			return new VariableDeclaration(variables);
		}
	}
}