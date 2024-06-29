using BMinus.Compiler;
using BMinus.Parser;
using BMinus.Tokenizer;
using BMinus.VirtualMachine;


string test = "auto a;a = 1+3;putchar(a);";
Parser p = new Parser(new Lexer(test));
var tree = p.Parse();
Compiler c = new Compiler(tree);
var env = c.GetEnvironment();
VirtualMachine vm = new VirtualMachine(env);
vm.Run();

