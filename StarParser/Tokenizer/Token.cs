namespace StarParser.Tokenizer;

public class Token
{
	public TokenType TokenType;
	public string Literal;
	//Position

	public Token(TokenType tt, string literal)
	{
		this.TokenType = tt;
		this.Literal = literal;
	}
}