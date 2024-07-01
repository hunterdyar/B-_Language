namespace BMinus.AST;

public class Statement
{
	public readonly uint UID;
	public Statement()
	{
		UID = Next();
	}

	public override string ToString()
	{
		return "";
	}
	
	
	//ID handling
	private static uint _lastId;

	public static uint Next()
	{
		_lastId++;
		return _lastId;
	}

	public static void ResetID()
	{
		_lastId = 1;
	}
}