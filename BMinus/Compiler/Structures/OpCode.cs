namespace BMinus.Compiler;

public enum OpCode : byte
{
	Halt,//stop
	Nop,//Do nothing
	SetReg,//puts op1 (any value) into op2 (register)
	SetLocal,//sets heap at pointer (op1) from register (op2)
	GetLocal,//puts pointer (op1) from heap into register (op2)
	Pop,//remove from stack, puts into EAX. pushing is done by setting register -1.
	SetGlobal,//sets global (index op1) from op2 (register)
	GetGlobal,//sets op2 (register) from global (index op 1)
	Bitwise,// A, B -> register op2. op1 is operator
	Arithmetic,//A, B -> register op2. op1 is operator
	Compare,//A,B -> register op2. op1 is operator
	GoTo,//jumps to frame (op1), instruction pointer (op2)
	Jump,//Jumps to instruction pointer (op1) in current frame
	JumpNotEq,//EAX, reads EAX, then does jump(op1,op2) if .. op3? hmmmm
	Return,//puts op1 in D and leaves frame.
	Call, //Pushes a new frame with instructions from frameprototype op1. arguments should be on stack.
}