using BMinus.VirtualMachine;

namespace BMinus.Compiler;

//aka runtime subroutine
//This can be rewritten without making copies of the instructions, but just runtime, holding the IP and environment.subroutines[frameID].instruction
public class Frame
{
	public SubroutineDefinition Source => _source;
	private SubroutineDefinition _source;
	public int FrameID;//this indexes to function names.
	public List<Instruction> Instructions => _source.Instructions;
	public int IP = -1;
	private int _stackBasePos;
	public int StackBasePos => _stackBasePos;
	public int ReturnRegister { get; set; }

	public int ArgCount;
	public int LocalVarCount;

	private VMRunner _runner;//static reference instead?
	public Frame(VMRunner runner)
	{
		FrameID = -1;
		_stackBasePos = 0;
		ArgCount = 0;
		LocalVarCount = 0;
		ReturnRegister = 0;
		_runner = runner;
	}

	public Frame(VMRunner runner, SubroutineDefinition prototype)
	{
		_runner = runner;
		_source = prototype;
		FrameID = prototype.FrameID;
		IP = -1;
		ArgCount = prototype.ArgumentCount;
		LocalVarCount = prototype.LocalCount;
		ReturnRegister = 0;
	}

	public Frame Clone()
	{
		return new Frame(_runner)
		{
			_source = this._source,
			FrameID = this.FrameID,
			IP = this.IP,
			_stackBasePos = this._stackBasePos,
			ArgCount = this.ArgCount,
			LocalVarCount = this.LocalVarCount,
			ReturnRegister = this.ReturnRegister
		};
	}

	public void SetIP(int ip)
	{
		this.IP = ip;
	}

	public void SetBasePointer(int pointer)
	{
		_stackBasePos = pointer;
	}
}