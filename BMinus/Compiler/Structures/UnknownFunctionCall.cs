using BMinus.VirtualMachine;

namespace BMinus.Compiler;

public struct UnknownFunctionCall
{
	public InstructionLocation? CallLocation;
	public string functionName;
	public SubroutineDefinition callingFrame;
	// public uint ASTID;
	private bool saveRestoreRegisterCheck = false;
	private InstructionLocation? save;
	private InstructionLocation? restore;
	public UnknownFunctionCall(InstructionLocation callLocation, string functionName, SubroutineDefinition callingFrame)
	{
		saveRestoreRegisterCheck = false;
		CallLocation = callLocation;
		this.functionName = functionName;
		this.callingFrame = callingFrame;
	}

	public UnknownFunctionCall(InstructionLocation callLocation, string functionName, SubroutineDefinition callingFrame, InstructionLocation? save, InstructionLocation? restore)
	{
		saveRestoreRegisterCheck = true;
		CallLocation = callLocation;
		this.functionName = functionName;
		this.callingFrame = callingFrame;
		this.save = save;
		this.restore = restore;
		Compiler.OnInstructionRemoved+= OnInstructionRemoved;
	}

	private void OnInstructionRemoved(int frameID, int insIndex)
	{
		//update these if needed.
		CallLocation = CallLocation?.OnOtherInstructionRemoved(frameID, insIndex);
		save = save?.OnOtherInstructionRemoved(frameID, insIndex);
		restore = restore?.OnOtherInstructionRemoved(frameID, insIndex);
	}

	public void TryToFindCallAgain(Compiler compiler)
	{
		if (!compiler.Subroutines.TryGetValue(functionName, out SubroutineDefinition? sub))
		{
			//todo: save the opcode location and name to a struct, and add to 'unknown functions' list/dictionary.
			//then, after, loop through all of them and try again.
			throw new CompilerException($"Unable to find function {functionName}");
		}
		//reference: Emit(OpCode.Call, fn.UID, sub.FrameID, VM.RET);
		callingFrame.UpdateOperands(CallLocation.Value,sub.FrameID,VirtualMachine.VirtualMachine.RET);
		
		
		if (saveRestoreRegisterCheck)
		{
			if (sub.ModifiedRegisters[VirtualMachine.VirtualMachine.A] || sub.ModifiedRegisters[VirtualMachine.VirtualMachine.B])
			{
				//if we don't modify the registers
				if (save != null)//it was already removed.
				{
					callingFrame.RemoveInstruction(save);
				}

				if (restore != null)//it was already removed
				{
					callingFrame.RemoveInstruction(restore);
				}
				//todo: this breaks the instruction locations that are saved in the compiler, and not just in the frame! uhg obviously. UHHHHHG.
				//hack a second 'original instruction locatin' to do a remap? might need this kind of thing for more complex compiler optimizations, but it feels like a wild overhead.
			}
		}
	}

	public override string ToString()
	{
		return $"Unknown '{functionName}' at {CallLocation}. S{save}. R{restore}";
	}
}