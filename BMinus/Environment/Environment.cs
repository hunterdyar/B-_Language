using BMinus.AST;
using BMinus.Compiler;
using BMinus.VirtualMachine;

namespace BMinus.Environment;

/// <summary>
/// The Environment is heap, and the ... frames.
/// </summary>
public class Environment
{
	public int FramePrototypeCount => _framePrototypes.Length;
	public Frame GetFramePrototype(int i) => _framePrototypes[i];
	private Frame[] _framePrototypes;
	public MemoryManager Memory => _memory;
	private MemoryManager _memory;

	private VMRunner _runner;
	public readonly Statement AST;

	public Environment(VMRunner runner,Statement root, Dictionary<string,int> globals, Frame[] framePrototypes)
	{
		AST = root;
		_memory = new MemoryManager(globals);
		_framePrototypes = framePrototypes;
		_runner = runner;
	}

	public int GetGlobal(int loc)
	{
		if(_memory.GetHeapValue(loc, out var val))
		{
			return val;
		}
		
		throw new VMException($"Unable to get global {loc}");
	}
	
	public void SetGlobal(int pos, int val)
	{
		var d = _memory.SetHeapValue(pos,val);
		_runner.OnHeapValueUpdated(pos,d);
	}


	public InstructionLocation GetLabel(int labelID)
	{
		return new InstructionLocation(0, 0);
	}
	
	public byte[] HeapMemorySegment()
	{
		return _memory.GetHeap();
	}
	
}