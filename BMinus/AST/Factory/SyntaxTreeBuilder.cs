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
				Console.WriteLine("Ooops! Expression as statement");
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
			case "ExpressionStatement":
				return WalkExpression(node.Children[0]);
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
				MustHaveChildrenCount(node, 1);
				return WalkExpression(node.Children[0]);
			case "InnerExpression":
				if (node.Children.Count == 1)
				{
					return WalkExpression(node.Children[0]);
				}else if (node.Children.Count == 2)
				{
					if (node.Children[0].Type == "PrefixOperator")
					{
						return PrefixOp.GetPrefixOp(WalkExpression(node.Children[1]), node.Children[0].Contents);
					}

					if (node.Children[1].Type == "InfixOperation")
					{
						var post = node.Children[1];
						var left = WalkExpression(node.Children[0]);
						var op = post.Contents;
						var rightNode = post.Children[0];
						if (rightNode != null)
						{
							op = rightNode.Contents;
							if (rightNode.Type == "BinaryOperator")
							{
								var right = WalkExpression(node.Children[1].Children[1]);
								return BinOp.GetBinaryOp(left, op, right);
							}
							else if (rightNode.Type == "TernaryOperator")
							{
								var consequence = WalkExpression((node.Children[1].Children[1]));
								var alternative = WalkExpression(node.Children[1].Children[2]);
							}
						}
					}
					if (node.Children[1].Type == "PostfixOperation")
                    {
                    	var post = node.Children[1];
                    	var left = WalkExpression(node.Children[0]);
                    	var op = post.Contents;
                    	var rightNode = post.Children[0];
                    	//return PostOp.GetPostOp()
	                    throw new NotImplementedException("Postfix Operator Parsing not implemented");
                    	return null;
                    }
				}else
				{
					//pre = 0,
					//expr = 1,
					//post = 2
					throw new Exception($"i don't know what to do with an expression with {node.Children.Count} children.");
					return null;
				}
				break;
			case "Identifier":
				return new Identifier(node.Contents);
			case "IntegerLiteral":
				return new WordLiteral(int.Parse(node.Contents));
			case "PostfixOperator":
				MustHaveChildrenCount(node, 2);
				if (node.Children[1].Type == "InfixOperation")
				{
					return BinOp.GetBinaryOp(WalkExpression(node.Children[0]),node.Children[1].Children[0].Contents,WalkExpression(node.Children[1]));
				}
				
				//return postfix other.
				break;
			case "PrefixOperator":
				return PrefixOp.GetPrefixOp(WalkExpression(node.Children[1]), node.Children[0].Contents);
			case "ParenthesizedExpression":
				MustHaveChildrenCount(node, 1);
				return WalkExpression(node.Children[0]);
		}

		throw new Exception($"I can't handle this node: {node}");
		return null;
	}

	private static void MustHaveChildrenCount(ParserTreeNode node, int i)
	{
		if (node.Children.Count != i)
		{
			throw new Exception($"Bad number of children in node! got {node.Children.Count}, expected {i}");
		}
	}

	
}