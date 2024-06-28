using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using BMinus.Compiler;

namespace BMinus.Environment;

/// <summary>
/// The Environment is heap, and the ... frames.
/// </summary>
public class Environment
{
	private List<int> _heap = new List<int>(2048); //initiate with some breathing room before we hitch to grow the internal array
	//list of frame primitives
	//list of constants, if we use those; otherwise the initial heap state
	public void SetHeap(int pointer, int value)
	{
		while (pointer > _heap.Count+1)
		{
			_heap.Add(0);
		}

		_heap[pointer] = value;
	}

	public bool GetHeap(int pointer, out int value)
	{
		if (pointer < 0 || pointer >= _heap.Count)
		{
			value = 0;
			return false;
		}
		value = _heap[pointer];
		return true;
	}
}