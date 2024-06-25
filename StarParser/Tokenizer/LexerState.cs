namespace StarParser.Tokenizer;

//wraps a lexer, so it never has to lex multiple times, and lets the parser backtrack between states.
public class LexerState
{
	private Lexer _lexer;
	private int pos;
	public LexerState(Lexer lexer)
	{
		_lexer = lexer;
	}

	public LexerState Clone()
	{
		return new LexerState(_lexer)
		{
			pos = this.pos
		};
	}
}