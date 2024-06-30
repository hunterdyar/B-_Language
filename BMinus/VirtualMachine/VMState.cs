namespace BMinus.VirtualMachine;

public enum VMState
{
	Ready,
	Error,
	Running,
	Stepping,
	Complete,
}