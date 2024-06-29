namespace BMinus.Compiler;

//aka runtime subroutine
public class Frame
{
	public int FrameID;//this indexes to function names.
	public Instruction[] Instructions;
	private int[] _locals;
	public int IP = -1;

	public Frame()
	{
		FrameID = -1;
		Instructions = null;
		_locals = null;
	}

	public Frame(SubroutineDefinition prototype)
	{
		FrameID = prototype.FrameID;
		Instructions = prototype.Instructions.ToArray();
		_locals = new int[prototype.Locals.Count];
		IP = -1;
	}

	#region Runtime

	public int GetLocal(int pos)
	{
		return _locals[pos];
	}

	public void SetLocal(int pos, int val)
	{
		_locals[pos] = val;
	}

	#endregion

	public Frame Clone()
	{
		return new Frame()
		{
			Instructions = this.Instructions,
			IP = this.IP,
			_locals = (int[])this._locals.Clone(),
		};
	}

	public void SetIP(int ip)
	{
		this.IP = ip;
	}
}