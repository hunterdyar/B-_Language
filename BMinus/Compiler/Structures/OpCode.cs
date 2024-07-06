namespace BMinus.Compiler;

public enum OpCode : byte
{
	Halt,//stop
	Nop,//Do nothing
	Move,//Sets value of op2 to value of op1.
	SetReg,//puts op1 (any value) into op2 (register)
	SetLocal,//sets stack at pointer (op1) from register (op2)
	GetLocal,//puts pointer (op1) from heap into register (op2)
	Pop,//remove from stack, puts into EAX. pushing is done by setting register -1.
	SetGlobal,//sets global (index op1) from op2 (register)
	GetGlobal,//sets op2 (register) from global (index op 1)
	Bitwise,// A, B -> register op2. op1 is operator
	Arithmetic,//A, B -> register op2. op1 is operator
	Compare,//A,B -> register op2. op1 is operator
	GoTo,//jumps to frame (op1), instruction pointer (op2)
	Jump,//Jumps to instruction pointer (op1) in current frame
	JumpNotZero,//reads X, then does jump(op1,op2) if .. x is nonzero.
	JumpZero, //reads X, then does jump(op1, op2) if x is zero.
	Return,//puts op1 in D and leaves frame.
	Call, //Pushes a new frame with instructions from frameprototype op1. arguments should be on stack.
	CallBuiltin,//calls c# code with stack as params. op1 is index of builtin, op2 is num of arguments to pop from stack.
	SaveRegister,//puts A and B onto the stck
	RestoreRegister,//restores B and A from the stack
}