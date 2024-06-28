namespace BMinus.Compiler;

public enum OpCode : byte
{
	Set,//puts op1 (any value) into op2 (register)
	SetGlobal,//sets global (index op1) from op2 (register)
	GetGlobal,//sets op2 (register) from global (index op 1)
	Bitwise,// A, B -> register op2. op1 is operator
	Arithmetic,//A, B -> register op2. op1 is operator
	Compare,//A,B -> register op2. op1 is operator
	GoTo,//jumps to frame (op1), instruction pointer (op2)
	Jump,//Jumps to instruction pointer (op1) in current frame
	JumpNotEq,//EAX, reads EAX, then does jump(op1,op2) if .. op3? hmmmm
	Return,//puts op1 in EAX and leaves frame.
	MoveReg,//sets op2 to op1 (op1->op2)
}