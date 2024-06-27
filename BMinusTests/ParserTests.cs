using StarParser.Parser;
using StarParser.Tokenizer;

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

			Parser p = new Parser(new Lexer(test));
			Actual = p.Parse().ToString();
			if (Actual == null)
			{
				Assert.Fail($"Unable to parse {test}."+p.ToString());
			}

			Assert.That(Actual, Is.EqualTo(expected));
			
		}
		
	}
	[SetUp]
	public void Setup()
	{
	}
	
	[Test]
	[TestCase("a=1;", "a = 1")]
	[TestCase("a=1+2;", "a = (1 + 2)")]
	public void AssignmentTest(string test, string expected)
	{
		var x = new ParseTest(test, expected);
	}	
	
	[Test]
	[TestCase("var a;", "var a")]
	[TestCase("var a, b, c;", "var a,b,c")]
	[TestCase("var a, b, c;", "var a,b,c")]
	[TestCase("var a,bee;", "var a,bee")]
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