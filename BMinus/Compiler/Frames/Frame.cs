namespace BMinus.Compiler;

public class Frame
{
	public int FrameID;
	public List<Instruction> Instructions = new List<Instruction>();

	public Frame(int frameId)
	{
		FrameID = frameId;
	}

	/// <summary>
	/// Adds an instruction and returns it's instructionLocation
	/// </summary>
	public InstructionLocation AddInstruction(Instruction instr)
	{
		Instructions.Add(instr);
		return new InstructionLocation(FrameID,Instructions.Count-1);
	}
}