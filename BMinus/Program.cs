
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using BMinus.Compiler;
using BMinus.Parser;
using BMinus.Tokenizer;
using BMinus.VirtualMachine;

Console.WriteLine("hi");

string test = "a = 1;";
Parser p = new Parser(new Lexer(test));
var tree = p.Parse();
Compiler c = new Compiler(tree);
var env = c.GetEnvironment();
VirtualMachine vm = new VirtualMachine(env);
vm.Run();
