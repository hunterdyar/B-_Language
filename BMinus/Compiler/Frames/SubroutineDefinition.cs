using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using BMinus.Models;

namespace BMinus.Compiler;

//aka 'compile-time'.
public class SubroutineDefinition
{
	public Dictionary<string, int> Locals = new Dictionary<string, int>();
	public Dictionary<string, int> Externs = new Dictionary<string, int>();

	public int FrameID;
	public readonly string Name;
	public List<Instruction> Instructions = new List<Instruction>();
	public readonly int ArgumentCount;
	public int LocalCount => Locals.Count;

	public SubroutineDefinition(string name,int id, int parameterCount)
	{
		this.Name = name;
		FrameID = id;
		ArgumentCount = parameterCount;
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
		Instructions[loc.InstructionIndex] = new Instruction(old.Op, old.ASTNodeID, operands);
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

	public void AddExtern(string name, int externIndex)
	{
		Externs.Add(name, externIndex);
	}

	public bool TryResolveID(string name, out int index, out Scope scope)
	{
		if (Locals.TryGetValue(name, out index))
		{
			scope = Scope.Local;
			return true;
		}

		if (Externs.TryGetValue(name, out index))
		{
			scope = Scope.Global;
			return true;
		}

		index = -1;
		scope = Scope.None;
		return false;
	}
}