// See https://aka.ms/new-console-template for more information

using BMinus.Parser;

var tokens = Tokenizer.Instance.Tokenize("1 * (2+3)");

foreach (var token in tokens)
{
	Console.WriteLine(token.Kind);	
}