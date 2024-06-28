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
}