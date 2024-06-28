namespace BMinus.AST;

public class VectorIdentifier : Identifier
{
	public Expression Size;

	
	//todo: right now vectors refer to their first instantiation as well as their accessed element.
	//so THATS a thing. but... technically variables do to. I think it makes sense.
	public VectorIdentifier(string id, Expression size) : base(id)
	{
		Size = size;
	}
	//The actual implementation of a vector consists of a word v which contains
	//in its right half the address of the 0-th word of the vector;
	// i.e., v is really a pointer to the actual vector
}