namespace BMinus.Tokenizer;

public enum TokenState
{
	Entry,
	Error,
	Complete,
	Identifier,
	Integer,
	HexInteger,
	String,
}