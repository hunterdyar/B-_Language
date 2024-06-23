﻿// See https://aka.ms/new-console-template for more information

using Ara3D.Parakeet;
using BMinus.Barakeet;
using BMinus.Parser;

ParserInput input = new ParserInput("var a;a=1+2");
var p = BMinusGrammar.Instance.Parse(input.Text);
var tree = p.Node.ToParseTree();

Console.WriteLine(tree);