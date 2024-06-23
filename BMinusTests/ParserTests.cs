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
		}
		
	}
	[SetUp]
	public void Setup()
	{
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