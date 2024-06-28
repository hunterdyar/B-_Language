namespace BMinus.VirtualMachine;

public class VirtualMachine
{
	public Environment.Environment Env;
	//Compiler gives us a compiler object, which is NOT really bytecode,
	//as we will have initial heap state (constants) and frame prototypes as objects.
}