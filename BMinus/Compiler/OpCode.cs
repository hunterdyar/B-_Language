namespace BMinus.Compiler;

public enum OpCode : byte
{
	SetReg,
	GetReg,
	SetGlobal,
	GetGlobal,
	Bitwise,
	Arithmetic,
	Compare,
	GoTo,
	Jump,
	JumpNotEq,
}