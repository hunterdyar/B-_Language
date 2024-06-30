namespace BMinus.VirtualMachine;

public enum VMState
{
	Ready = 0,
	Error = 4,
	Running = 1,
	Stepping = 2,
	Complete = 3,
	Uninitialized = 5,
}