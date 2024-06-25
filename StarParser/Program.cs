// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text;
using StarParser.Parser;
using StarParser.Tokenizer;

Console.WriteLine("Lexing!");

//first, write a normal tokenizer
//then, write a backtracking recursive descent parser.
//why backtracking? fuck you, that's why.

var input = "(a)";
var l = new Lexer(input);
StringBuilder sb = new StringBuilder();
for (int i = 0; i < l.Tokens.Count; i++)
{
	sb.Append(l.Tokens[i]);
	if (i < l.Tokens.Count - 1)
	{
		sb.Append(", ");
	}
}
Console.WriteLine(sb.ToString());
Console.WriteLine("Parsing!");
var root = Parser.Parse(l);
Console.WriteLine(root);
var ast = root.GetASTNode();
Console.WriteLine(ast);