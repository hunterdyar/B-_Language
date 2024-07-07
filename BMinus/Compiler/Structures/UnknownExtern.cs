using System.Security.AccessControl;

namespace BMinus.Compiler;

public class UnknownExtern
{
	public string externName;
	private int register;
	public SubroutineDefinition callingFrame;
	private InstructionLocation _instructionLocation;

	public UnknownExtern(string externName, int register, InstructionLocation location,
		SubroutineDefinition callingFrame)
	{
		this.externName = externName;
		this.callingFrame = callingFrame;
		this.register = register;
		this._instructionLocation = location;
	}

	public void TryToFindExternAgain(Compiler compiler)
	{
		int index = compiler.Globals.IndexOf(externName);
		if (index != -1)
		{
			//Emit(OpCode.GetGlobal, expression.UID, index, register);
			callingFrame.UpdateOperands(_instructionLocation, index, register);
		}
		else
		{
			throw new CompilerException($"Unable to resolve extern {externName}");
		}
	}
}