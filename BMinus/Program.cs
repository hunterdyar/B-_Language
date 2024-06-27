// See https://aka.ms/new-console-template for more information

using Ara3D.Parakeet;
using BMinus.AST;
using BMinus.Barakeet;


ParserInput input = new ParserInput("1+2+3;");
try
{
	var p = BMinusGrammar.Instance.Parse(input.Text);
	if (p == null)
	{
		throw new Exception("Bad");
	}
	Console.WriteLine(p.Node.ToParseTree());
	var tree = SyntaxTreeBuilder.WalkStatement(p.Node.ToParseTree());
	Console.WriteLine(tree);
}
catch (Exception e)
{
	Console.WriteLine(e);
	throw;
}

