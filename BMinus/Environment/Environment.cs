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
	
	public readonly Statement AST;
	public Environment(Statement root, List<string> globals, Frame[] framePrototypes)
	{
		AST = root;
		_memory = new MemoryManager(globals);
		_framePrototypes = framePrototypes;
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
		_memory.SetHeapValue(pos,val);

	}

	public void SetLocal(Frame frame, int loc, int val)
	{
		_memory.SetHeapValue(frame.BasePointer+loc,val);
	}

	public int GetLocal(Frame frame, int loc)
	{
		if (_memory.GetHeapValue(frame.BasePointer+loc, out var val))
		{
			return val;
		}

		throw new VMException($"Unable to get global {loc} for frame {frame.FrameID}");
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