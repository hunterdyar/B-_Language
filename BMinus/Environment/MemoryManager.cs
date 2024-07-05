using System.ComponentModel;
using BMinus.VirtualMachine;

namespace BMinus.Environment;

public class MemoryManager
{
	private byte[] _memory = new byte[256];
	private byte[] _exportCache;
	private int _topLoc;
	private const int WordSize = 4;
	private List<(int, int)> _frames = new List<(int, int)>();
	//cache to prevent lots of _frames.Count
	private (int, int) _topFrame;
	public MemoryManager(List<string> initialVariables)
	{
		_topFrame = new (0,initialVariables.Count);
		_frames.Add(_topFrame);
		//set size to initialvariables, at least.
		//store names in out lookup table used for debugging.
	}

	public int LocalFrameIndex => _frames.Count;

	public void PushFrame(int initSize)
	{
		var b = _topLoc + WordSize;//NextAvailable
		_topFrame = (b, initSize);
		_topLoc = _topLoc + (initSize * WordSize);
		_frames.Add(_topFrame);
		
	}

	public void RemoveTopFrame()
	{
		if (_frames.Count == 1)
		{
			throw new VMException("Shouldn't remove last frame, thats where the globals are.");
		}
		ReduceSize(_topFrame.Item1 - WordSize);
		_frames.RemoveAt(_frames.Count-1);
		//set new topframe cache
		_topFrame = _frames[^1];
	}

	public byte[] SetLocal(int pos, int value)
	{
		var f = _frames[^1];
		if (f.Item2 > pos)
		{
			_frames[^1] = (f.Item1, pos);
			_topLoc = f.Item1 + f.Item2;//this? should be true?
		}

		return SetHeapValue(f.Item1 + pos, value);
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

	public int GetLocal(int pos)
	{
		if (GetHeapValue(_frames[^1].Item1 + pos, out var val))
		{
			return val;
		}

		if (_frames.Count == 0)
		{
			throw new VMException("Can't get local, no frames.");
		}

		throw new VMException($"Unable to get heap value {pos}");
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

	public void ReduceSize(int newTop)
	{
		
		if (newTop <= _topLoc)
		{
			_topLoc = newTop;
			return;
		}

		if (_topLoc == 0)
		{
			throw new VMException("Can't reduce size, there is no data");
		}
		
		throw new VMException($"Unable to reduce size of heap. asked for new: {newTop}, old is {_topLoc}");
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