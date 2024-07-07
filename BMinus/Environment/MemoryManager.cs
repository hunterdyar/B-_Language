using System.ComponentModel;
using BMinus.VirtualMachine;

namespace BMinus.Environment;

//todo: get rid of integer pointers and replace entirely with "memory locations".
public class MemoryManager
{
	private byte[] _memory = new byte[256];
	private byte[] _exportCache;
	private int _lastUsed = -1;
	private const int WordSize = sizeof(int);
	public MemoryManager(Dictionary<string,int> initialVariables)
	{
		_lastUsed = initialVariables.Count * WordSize - 1;
		//it's okay if last used is -1. 
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

	public int GetFreeAndMarkUsed(int size = WordSize)
	{
		var p = _lastUsed+1;//lets say lastused is 4. We return 5, and mark 5,6,7,and 8 (sizeof int is 4) as used.
		_lastUsed += WordSize;
		return p;
	}
	//list of frame primitives
	//list of constants, if we use those; otherwise the initial heap state
	//change type here from int to Word object, which we don't have defined yet (it's an int!)
	//we could overload this with any value besides int, that BitConverter uses.
	public byte[] SetHeapValue(int pointer, int value)
	{
		while (pointer > _memory.Length)//3 is the rest of the bytes 
		{
			DoubleMemoryContainer();
		}
		
		var data = IntToBytes(value);
		
		data.CopyTo(_memory,pointer);
		
		if (pointer > _lastUsed)
		{
			_lastUsed = pointer + WordSize-1;//if we set x to an int, x,x+1,x+2, and x+3 get used.
		}

		return data;
	}
	
	public bool GetHeapValue(int pointer, out int value)
	{
		if (pointer < 0 || pointer > _lastUsed)
		{
			value = 0;
			return false;
		}
		
		var dataBytes = new ArraySegment<byte>(_memory,pointer ,WordSize);

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
		if (_exportCache == null || _exportCache.Length != _lastUsed+1)
		{
			_exportCache = new byte[_lastUsed+1];
		}
		//return all
		_exportCache = new ArraySegment<byte>(_memory, 0, _lastUsed + 1).ToArray();
		return _exportCache;
	}
}