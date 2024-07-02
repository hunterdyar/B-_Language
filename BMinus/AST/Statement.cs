using System.Runtime.CompilerServices;
using System.Text;

namespace BMinus.AST;

public class Statement
{
	public readonly uint UID;
	public string Label;
	public Statement()
	{
		UID = Next();
	}

	public override string ToString()
	{
		return "";
	}

	public virtual string GetJSON()
	{
		return "{\"name\": \"" + GetJSONName() + "\",\"id\": " + UID + ",\"label\": \"" + Label + "\",\"children\":" +
		       GetJSONChildren()
		       +"}";
	}

	protected virtual string GetJSONName()
	{
		return this.GetType().Name;
	}

	protected virtual string GetJSONChildren()
	{
		StringBuilder sb = new StringBuilder();
		sb.Append('[');
		bool atLeastOne = false;
		foreach (var s in GetChildren())
		{
			sb.Append(s.GetJSON());
			sb.Append(',');
			atLeastOne = true;
		}
		//remove last comma
		if (atLeastOne)
		{
			sb.Remove(sb.Length - 1, 1);
		}
		sb.Append(']');
		return sb.ToString();
	}
	protected virtual IEnumerable<Statement> GetChildren()
	{
		return ArraySegment<Statement>.Empty;
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