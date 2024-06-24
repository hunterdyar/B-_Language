// See https://aka.ms/new-console-template for more information

using Ara3D.Parakeet;
using BMinus.AST;
using BMinus.Barakeet;


ParserInput input = new ParserInput("var  a;a=b+c;");
try
{
	var p = BMinusGrammar.Instance.Parse(input.Text);
	var tree = SyntaxTreeBuilder.WalkStatement(p.Node.ToParseTree());
	Console.WriteLine(tree);
}
catch (ParserException e)
{
	Console.WriteLine(e);
	throw;
}

