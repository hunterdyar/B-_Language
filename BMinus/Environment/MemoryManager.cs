using System.ComponentModel;
using BMinus.VirtualMachine;

namespace BMinus.Environment;

//todo: get rid of integer pointers and replace entirely with "memory locations".
public class MemoryManager
{
	private byte[] _memory = new byte[256];
	private byte[] _exportCache;
	private int _topLoc;
	private const int WordSize = 4;
	public MemoryManager(List<string> initialVariables)
	{
		
	}
	
	public static byte[] IntToBytes(int value)
	{
		var data = BitConverter.GetBytes(value);
		if (!BitConverter.IsLittleEndian)
		{
			data = data.Reverse().ToArray();
		}

		return data;
	}

	private int HeapFromPointer(int pointer)
	{
		//using 4 bytes for all words (for now!)
		return pointer * WordSize;
	}
	//list of frame primitives
	//list of constants, if we use those; otherwise the initial heap state
	public byte[] SetHeapValue(int pointer, int value)
	{
		var loc = HeapFromPointer(pointer);

		while (loc + (WordSize-1) > _memory.Length)//3 is the rest of the bytes 
		{
			DoubleMemoryContainer();
		}

		var data = IntToBytes(value);
		
		data.CopyTo(_memory,loc);
		
		if (loc > _topLoc)
		{
			_topLoc = loc;
		}

		return data;
	}

	public int GetNextAvailablePointer()
	{
		return _topLoc + WordSize;
	}
	
	public bool GetHeapValue(int pointer, out int value)
	{
		var loc = HeapFromPointer(pointer);
		if (pointer < 0 || loc > _topLoc)
		{
			value = 0;
			return false;
		}
		
		var dataBytes = new ArraySegment<byte>(_memory,loc ,WordSize);

		if (!BitConverter.IsLittleEndian)
		{
			dataBytes = dataBytes.Reverse().ToArray();
		}
		value = BitConverter.ToInt32(dataBytes);
		return true;
	}

	private void DoubleMemoryContainer()
	{
		var current = _memory.Length;
		var next = current * 2;
		var newMemory = new byte[next];
		_memory.CopyTo(newMemory,0);
		_memory = newMemory;
	}

	public byte[] GetHeap()
	{
		if (_exportCache == null || _exportCache.Length != _topLoc+WordSize-1)
		{
			_exportCache = new byte[_topLoc + WordSize];
		}
		//return all
		_exportCache = new ArraySegment<byte>(_memory, 0, _topLoc + WordSize).ToArray();
		return _exportCache;
	}
}