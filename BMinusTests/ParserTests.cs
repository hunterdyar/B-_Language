using BMinus.Parser;
using BMinus.Tokenizer;

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
			var tree = p.Parse();
			Actual = tree.ToString();
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
	[TestCase("b = !a;", "b = !a")]
	[TestCase("b =!(1+2);", "b = !(1 + 2)")]
	[TestCase("((((a))));", "a")]

	public void PrefixOperatorTest(string test, string expected)
	{
		var x = new ParseTest(test, expected);
	}

	[Test]
	[TestCase("1+2+3;", "((1 + 2) + 3)")]
	[TestCase("a?1:0;", "a ? 1 : 0")]

	public void InfixOperatorTest(string test, string expected)
	{
		var x = new ParseTest(test, expected);
	}

	[Test]
	[TestCase("main(){}", "main(){}")]
	[TestCase("main(a){}", "main(a){}")]
	[TestCase("main(a,b,c){}", "main(a,b,c){}")]
	[TestCase("main(){a;}", "main(){a;}")]
	[TestCase("main();","main()")]
	[TestCase("main(1);", "main(1)")]
	[TestCase("main(1+2);", "main((1 + 2))")]
	[TestCase("a(b(c));", "a(b(c))")]
	public void FuctionTest(string test, string expected)
	{
		var x = new ParseTest(test, expected);
	}

	[Test]
	[TestCase("if(a){b;}", "if(a){b;}")]
	[TestCase("if(a){b;}else{c;}", "if(a){b;} else {c;}")]
	[TestCase("if(a){b;}else if(c){d;}", "if(a){b;} else if(c){d;}")]

	public void IfTest(string test, string expected)
	{
		var x = new ParseTest(test, expected);
	}
	
	//todo: test labels and gotos. make sure that the colon doesn't break ternarys. may need to move that logic into lexer so whitespace can differentiate.
}