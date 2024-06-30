using System.Diagnostics;
using BMinus.Compiler;
using BMinus.Parser;
using BMinus.Tokenizer;
using BMinus.VirtualMachine;


string test = "auto a;a = 1+3*2;putchar(a);";
Stopwatch _stopwatch = Stopwatch.StartNew();
Parser p = new Parser(new Lexer(test));
var tree = p.Parse();
Compiler c = new Compiler();
c.NewCompile(tree);
_stopwatch.Stop();
Console.WriteLine($"Compiled in {_stopwatch.ElapsedMilliseconds}ms");
var env = c.GetEnvironment();
VirtualMachine vm = new VirtualMachine(env);
vm.Run();
