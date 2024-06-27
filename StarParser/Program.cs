// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text;
using StarParser.Parser;
using StarParser.Tokenizer;

//first, write a normal tokenizer
//then, write a backtracking recursive descent parser.
//why backtracking? fuck you, that's why.

var input = "12;a;";
// var l = new Lexer(input);
Console.WriteLine("Parsing!");

Parser.TryParse(input, out var statement, out var err, out var errPos);
if (statement != null)
{
	Console.WriteLine(statement);
}
else
{
	Console.WriteLine(err);
}
