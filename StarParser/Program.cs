using BMinus.Parser;
using BMinus.Tokenizer;

var input = """
            main(a){a;}
            main(12+2);
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
