
using System.Globalization;
using System.IO.Compression;
using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Statement = BMinus.AST.Statement;
namespace BMinus.Parser;

public class BMParser
{
	public static Statement Parse(string program)
	{
		var b = TryParse(program, out var root, out var err);
		if (!b)
		{
			Console.WriteLine(err);
		}
		return root;
	}

	public static bool TryParse(string program, out Statement root, out string error)
	{
		error = "";
		if (string.IsNullOrEmpty(program))
		{
			error = "Program string is empty";
			root = new Nop(new Position());
			return false;
		}

		var tokenList = Tokenizer.Instance.TryTokenize(program);
		if (!tokenList.HasValue)
		{
			error = tokenList.ToString();
			root = new Nop(new Position());
			return false;
		}

		var result = StatementParser.Parser.Parse(tokenList.Value);
		root = result;
		return true;
	}
	
	

	// static readonly TokenListParser<BMToken, Expression> HexNumber =
	// 	Token.EqualTo(BMToken.HexLiteral)
	// 		.Apply(Tokenizer.HexInteger)
	// 		.Select(n => ulong.Parse(n, NumberStyles.HexNumber, CultureInfo.InvariantCulture)
	// 		.Select(u => (Expression)new WordLiteral(n,u);

	// private static readonly TokenListParser<BMToken, Expression> HexNumber =
		// from token in Token.EqualTo(BMToken.HexLiteral)
		// from p in ulong.Parse(token.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture)
		// select new WordLiteral(token, p);
}