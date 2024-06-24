using Ara3D.Parakeet;
using BMinus.AST;
using BMinus.Barakeet;

namespace BMinusTests;

public class Tests
{
	public class ParseTest
	{
		public string Expected;
		public string Test;
		public string Actual;

		public ParseTest(string test, string expected)
		{
			Actual = "";
			Expected = expected;
			Test = test;
			// var p = BMParser.TryParse(test,out var s, out var e);
			// if (!p)
			// {
			// 	Assert.Fail(e);
			// }
			//
			// var actual = s.ToString();
			// Actual = actual;
			// Assert.AreEqual(expected, actual);
			var p = BMinusGrammar.Instance.Parse(test);
			if (p == null)
			{
				Assert.Fail($"Unable to parse {test}");
				return;
			}
			
			if (!p.AtEnd())
			{
				Assert.Fail();
				return;
			}

			var t = p.Node.ToParseTree();
			Console.WriteLine(t);
			var tree = SyntaxTreeBuilder.WalkStatement(t);
			if(tree == null){Assert.Fail();}
			Assert.That(tree?.ToString(), Is.EqualTo(expected));
		}
		
	}
	[SetUp]
	public void Setup()
	{
	}
	
	[Test]
	[TestCase("a=1", "a = 1")]
	[TestCase("a=1+2", "a = (1 + 2)")]
	public void AssignmentTest(string test, string expected)
	{
		var x = new ParseTest(test, expected);
	}	
	
	[Test]
	[TestCase("var a;", "var a")]
	[TestCase("var a, b, c", "var a,b,c")]
	[TestCase("var a, b, c;", "var a,b,c")]
	[TestCase("var a,bee,;", "var a,bee")]
	public void DeclarationTest(string test, string expected)
    {
    	var x = new ParseTest(test,expected);
    }

	[Test]
	[TestCase("b = !a", "b = !a")]
	[TestCase("b =!(1+2)", "b = !(1 + 2)")]

	public void PrefixOperatorTest(string test, string expected)
	{
		var x = new ParseTest(test, expected);
	}

	[Test]
	[TestCase("1+2+3;", "((1+2)+3)")]
	[TestCase("a?1:0;", "a ? 1 : 0")]

	public void InfixOperatorTest(string test, string expected)
	{
		var x = new ParseTest(test, expected);
	}	
}