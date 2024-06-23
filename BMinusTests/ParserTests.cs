using BMinus.Parser;
using Superpower.Display;

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
			var p = BMParser.TryParse(test,out var s, out var e);
			if (!p)
			{
				Assert.Fail(e);
			}

			var actual = s.ToString();
			Actual = actual;
			Assert.AreEqual(expected, actual);
		}
		
	}
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void IdentifierTest()
	{
		var validID = new[] { "a", "hello", "thisIsAnID", "a123", "a_b_c" };
		foreach (string id in validID)
		{
			var r = Tokenizer.Instance.TryTokenize(id);
			if (r.HasValue)
			{
				var t = r.Value;
				Assert.That(t.Count(), Is.EqualTo(1));
				foreach (var token in t)
				{
					if (token.Kind == BMToken.Identifier)
					{
						Assert.That(id, Is.EqualTo(token.Span.ToString()));
					}
					else
					{
						Assert.Fail();
					}
				}
			}
			else
			{
				Assert.Fail($"Unable to Tokenize {id}. {r.ErrorMessage}");
			}
		}
	}
	[Test]
	public void AssignmentTest()
	{
		var x = new ParseTest("a=1", "a = 1");
		var y = new ParseTest("a=1+2", "a = (1 + 2)");
	}
	[Test]
	public void DeclarationTest()
    {
    	var x = new ParseTest("var a;", "var a");
    	var y = new ParseTest("var a, b, c", "var a,b,c,");
	    // x = new ParseTest("var a,b,c,", "var a,b,c,");
    }	
}