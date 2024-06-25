// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text;
using StarParser.Parser;
using StarParser.Tokenizer;

Console.WriteLine("Lexing!");

//first, write a normal tokenizer
//then, write a backtracking recursive descent parser.
//why backtracking? fuck you, that's why.

var input = "f(a){var b = 0!=2}";
var l = new Lexer(input);
StringBuilder sb = new StringBuilder();
var t = l.NextToken();
while (t.TokenType != TokenType.End)
{
	sb.Append(t.ToString());
	sb.Append(", ");
	t = l.NextToken();
}
Console.WriteLine(sb.ToString());
Console.WriteLine("Parsing!");
var root = Parser.Parse(l);
Console.WriteLine(root);
var ast = root.GetASTNode();
Console.WriteLine(ast);