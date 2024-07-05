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

	public Action<int, int, byte[]> OnValueUpdated;
	public Environment(VMRunner runner,Statement root, List<string> globals, Frame[] framePrototypes)
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
		_runner.OnValueUpated(0,pos,d);
	}

	public void SetLocal(int loc, int val)
	{
		var d = _memory.SetLocal(loc,val);
		_runner.OnValueUpated(_memory.LocalFrameIndex, loc, d);
	}

	public int GetLocal(int loc)
	{
		return _memory.GetLocal(loc);
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