using BMinus.Compiler;
using BMinus.VirtualMachine;

namespace BMinus.Environment;

/// <summary>
/// The Environment is heap, and the ... frames.
/// </summary>
public class Environment
{
	public Frame GetFramePrototype(int i) => _framePrototypes[i];
	private Frame[] _framePrototypes;
	public MemoryManager Memory => _memory;
	private MemoryManager _memory;

	//dictionaries are temp till i do other stuff.
	private Dictionary<int, int> _globals = new Dictionary<int, int>();
	private Dictionary<int, int> _heap = new Dictionary<int, int>();
	public Environment(List<string> globals, Frame[] framePrototypes)
	{
		_memory = new MemoryManager(globals);
		_framePrototypes = framePrototypes;
	}

	public int GetGlobal(int loc)
	{
		if (_globals.TryGetValue(loc, out var val))
		{
			return val;
		}

		throw new VMException($"Unable to get global {loc}");
	}

	public void SetGlobal(int pos, int val)
	{
		if (_globals.ContainsKey(pos))
		{
			_globals[pos] = val;
		}
		else
		{
			_globals.Add(pos,val);
		}
	}

	public InstructionLocation GetLabel(int labelID)
	{
		return new InstructionLocation(0, 0);
	}
}