namespace BMinus.VirtualMachine;

public class VMException : Exception
{
	public VMException(string message) : base(message){}
}