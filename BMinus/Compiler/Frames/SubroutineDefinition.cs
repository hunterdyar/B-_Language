using BMinus.AST;
using BMinus.AST.PrimitiveStatements;

namespace BMinus.Compiler;

//aka 'compile-time'.
public class SubroutineDefinition
{
	public Dictionary<string, int> Locals = new Dictionary<string, int>();
	public int FrameID;
	public List<Instruction> Instructions = new List<Instruction>();
	
	public SubroutineDefinition(int id)
	{
		FrameID = id;
	}
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

	public void AddLocal(string id)
	{
		if (Locals.ContainsKey(id))
		{
			throw new CompilerException($"Local named {id} already exists!");
		}
		Locals.Add(id,Locals.Count);
	}

	public bool TryGetLocal(string name, out int index)
	{
		return Locals.TryGetValue(name, out index);
	}
}