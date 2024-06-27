using StarParser.Parser;
using StarParser.Tokenizer;

var input = """
            a = 1+2*3-0+1+2+2;
            """;
Console.WriteLine("B-");

Parser p = new Parser(new Lexer(input));
var statement = p.Parse();
if (statement != null)
{
	Console.WriteLine(statement);
}
else
{
	Console.WriteLine("oopsy");
}
