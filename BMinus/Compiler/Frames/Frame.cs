namespace BMinus.Compiler;

//aka runtime subroutine
public class Frame
{
	public int FrameID;//this indexes to function names.
	public Instruction[] Instructions;
	public int IP = -1;
	private int _basePointer;
	public int BasePointer => _basePointer;

	
	public Frame()
	{
		FrameID = -1;
		Instructions = null;
	}

	public Frame(SubroutineDefinition prototype)
	{
		FrameID = prototype.FrameID;
		Instructions = prototype.Instructions.ToArray();
		IP = -1;
	}


	public Frame Clone()
	{
		return new Frame()
		{
			FrameID = this.FrameID,
			Instructions = this.Instructions,
			IP = this.IP,
			_basePointer = this._basePointer,
		};
	}

	public void SetIP(int ip)
	{
		this.IP = ip;
	}

	public void SetBasePointer(int pointer)
	{
		_basePointer = pointer;
	}
}