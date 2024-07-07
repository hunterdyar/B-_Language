using BMinus.VirtualMachine;

namespace BMinus.Compiler;

public struct UnknownFunctionCall
{
	public InstructionLocation CallLocation;
	public string functionName;
	public SubroutineDefinition callingFrame;
	// public uint ASTID;

	public UnknownFunctionCall(InstructionLocation callLocation, string functionName, SubroutineDefinition callingFrame)
	{
		CallLocation = callLocation;
		this.functionName = functionName;
		this.callingFrame = callingFrame;
	}

	public void TryToFindCallAgain(Compiler compiler)
	{
		if (!compiler.Subroutines.TryGetValue(functionName, out SubroutineDefinition? sub))
		{
			//todo: save the opcode location and name to a struct, and add to 'unknown functions' list/dictionary.
			//then, after, loop through all of them and try again.
			throw new CompilerException($"Unable to find function {functionName}");
		}
		//Emit(OpCode.Call, fn.UID, sub.FrameID, VM.RET);
		callingFrame.UpdateOperands(CallLocation,sub.FrameID,VirtualMachine.VirtualMachine.RET);
	}
}