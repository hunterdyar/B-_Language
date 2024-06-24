using Ara3D.Parakeet;
using Ara3D.Parakeet.Cst.CSharpGrammarNameSpace;
using BMinus.AST.PrimitiveStatements;

namespace BMinus.AST;

public static class SyntaxTreeBuilder
{
	public static Statement WalkStatement(ParserTreeNode node)
	{
		switch (node.Type)
		{
			case "Program":
				List<Statement> children = new List<Statement>();
				foreach (var n in node.Children)
				{
					children.Add(WalkStatement(n));
				}
				return new ProgramStatement(children);
			case "Statement":
				return WalkStatement(node.Children[0]);
			case "Expression":
				return WalkExpression(node.Children[0]);
			
			case "Assignment":
				return new Assignment(WalkExpression(node.Children[0]), WalkExpression(node.Children[1]));
			case "VariableDeclaration":
				var identifiers = new List<Expression>();
				foreach (var c in node.Children)
				{
					identifiers.Add(new Identifier(c.Contents));
				}

				return new VariableDeclaration(identifiers.ToArray());
			default:
				break;
		}

		return new Nop();
	}

	public static Expression WalkExpression(ParserTreeNode node)
	{
		switch (node.Type)
		{
			case "Expression":
				if (node.Children.Count == 1)
				{
					return WalkExpression(node.Children[0]);
				}else if (node.Children.Count == 2)
				{
					if (node.Children[0].Type == "PrefixOperator")
					{
						return null;
					}

					if (node.Children[1].Type == "PostfixOperator")
					{
						var post = node.Children[1];
						var left = WalkExpression(node.Children[0]);
						var op = post.Contents;
						var rightNode = post.Children[0];
						if (rightNode != null && rightNode.Type == "BinaryOperation")
						{
							op = rightNode.Children[0].Contents;
							var right = WalkExpression(rightNode.Children[1]);
							return BinOp.GetBinaryOp(left, op, right);
						}
						//return PostOp.GetPostOp()
						return null;
					}
				}else if (node.Children.Count == 3)
				{
					//pre = 0,
					//expr = 1,
					//post = 2
					return null;
				}
				break;
			case "Identifier":
				return new Identifier(node.Contents);
			case "IntegerLiteral":
				return new WordLiteral(int.Parse(node.Contents));
			case "PostfixOperator":
				if (node.Children[1].Type == "BinaryOperation")
				{
					return BinOp.GetBinaryOp(WalkExpression(node.Children[0]),node.Children[1].Children[0].Contents,WalkExpression(node.Children[1]));
				}
				
				//return postfix other.
				break;
			case "PrefixOperator":
				return PrefixOp.GetPrefixOp(WalkExpression(node.Children[1]), node.Children[0].Contents);
		}

		return null;
	}
}