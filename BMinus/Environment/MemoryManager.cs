namespace BMinus.Environment;

public class MemoryManager
{
	private List<int> _heap = new List<int>(2048); //initiate with some breathing room before we hitch to grow the internal array

	public MemoryManager(List<string> initialVariables)
	{
		//set size to initialvariables, at least.
		//store names in out lookup table used for debugging.
	}

	//list of frame primitives
	//list of constants, if we use those; otherwise the initial heap state
	public void SetHeap(int pointer, int value)
	{
		while (pointer > _heap.Count + 1)
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