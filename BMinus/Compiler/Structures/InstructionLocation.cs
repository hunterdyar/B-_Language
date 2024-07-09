namespace BMinus.Compiler;

public struct InstructionLocation
{
	public int FrameIndex;
	public int InstructionIndex;

	public InstructionLocation(int frameIndex, int instructionIndex)
	{
		FrameIndex = frameIndex;
		InstructionIndex = instructionIndex;
	}

	//call when a different instruction is removed.
	public InstructionLocation? OnOtherInstructionRemoved(int frameId, int insIndex)
	{
		if (frameId == this.FrameIndex)
		{
			if (insIndex < this.InstructionIndex)
			{
				return new InstructionLocation(frameId, InstructionIndex--);
			}else if (insIndex == InstructionIndex)
			{
				return null;
			}
		}

		return this;
	}
}