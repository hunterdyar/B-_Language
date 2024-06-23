// See https://aka.ms/new-console-template for more information

using BMinus.Parser;

var tokens = Tokenizer.Instance.Tokenize("1 * (2+3)");

foreach (var token in tokens)
{
	Console.WriteLine(token.Kind);	
}


//I think I want to switch to pegasus parser.
var parser = new PegExamples.ExpressionParser();
var result = parser.Parse("5.1+2*3");
Console.WriteLine(result);

var p = new BMinus.Parser.Parser();
var r = p.Parse("123");
Console.WriteLine(r);