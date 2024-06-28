namespace BMinus.Compiler;

public class Frame
{
	public int FrameID;//this indexes to function names.
	public List<Instruction> Instructions = new List<Instruction>();

	public Frame(int frameId)
	{
		FrameID = frameId;
	}

	#region CompileTime
	/// <summary>
	/// Adds an instruction and returns it's instructionLocation
	/// </summary>
	public InstructionLocation AddInstruction(Instruction instr)
	{
		Instructions.Add(instr);
		return GetTopInstructionLocation();
	}

	public InstructionLocation GetTopInstructionLocation()
	{
		return new InstructionLocation(FrameID, Instructions.Count - 1);
	}

	public void UpdateOperands(InstructionLocation loc, params int[] operands)
	{
		if (loc.FrameIndex != FrameID)
		{
			throw new CompilerException("Unable to update instructions, wrong frame!");
		}

		var old = Instructions[loc.InstructionIndex];
		Instructions[loc.InstructionIndex] = new Instruction(old.Op, operands);
	}
	#endregion

	#region Runtime

	#endregion
}