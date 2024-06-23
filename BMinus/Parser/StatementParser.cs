using BMinus.AST;
using BMinus.AST.PrimitiveStatements;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Statement = BMinus.AST.Statement;
namespace BMinus.Parser;

public class StatementParser
{
	
	public static readonly TokenListParser<BMToken, Statement> SingleVariableDeclarationParser = 
     		from var in Token.EqualTo(BMToken.VarKeyword)
     		from id in ExpressionParsers.IdentifierParser
     		select (Statement)new VariableDeclaration(id, var.Position);
	
	public static readonly TokenListParser<BMToken, Statement> MultipleVariableDeclarationParser =
		from var in Token.EqualTo(BMToken.VarKeyword)
		from ids in ExpressionParsers.IdentifierParser.ManyDelimitedBy(Token.EqualTo(BMToken.Comma), Token.EqualTo(BMToken.Comma))
		select (Statement)new VariableDeclaration(ids, var.Position);

	public static readonly TokenListParser<BMToken, Statement> VariableDeclarationParser =
		from dec in MultipleVariableDeclarationParser.Or(SingleVariableDeclarationParser)
		select dec;
	
	// public static readonly TokenListParser<BMToken, Statement> AssignmentParser =
	// 		from id in ExpressionParsers.IdentifierParser
	// 		from eq in Token.EqualTo(BMToken.Assign)
	// 		from exp in ExpressionParsers.ExpressionParser
	// 		select (Statement)new Assignment(id, exp, eq.Position);

	public static readonly TokenListParser<BMToken, Statement> StmntParser =
		from s in VariableDeclarationParser
		// from s in AssignmentParser.Or(VariableDeclarationParser)
		select s;
	
	public static readonly TokenListParser<BMToken, Statement> Parser = 
		from p in StmntParser
		// from p in StmntParser.AtLeastOnceDelimitedBy(Token.EqualTo(BMToken.EndExpression))
			// .AtEnd()
		select (Statement)new ProgramStatement(p);
}