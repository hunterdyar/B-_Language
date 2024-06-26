// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text;
using StarParser.Parser;
using StarParser.Tokenizer;

//first, write a normal tokenizer
//then, write a backtracking recursive descent parser.
//why backtracking? fuck you, that's why.

var input = "a = 12;b=15;";
var l = new Lexer(input);
Console.WriteLine("Parsing!");
var root = Parser.Parse(l);
Console.WriteLine(root);
