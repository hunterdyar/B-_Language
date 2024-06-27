using StarParser.Parser;
using StarParser.Tokenizer;

var input = "a = a*b+c;";
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
