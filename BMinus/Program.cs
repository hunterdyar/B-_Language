using System.Diagnostics;
using BMinus.Compiler;
using BMinus.Parser;
using BMinus.Tokenizer;
using BMinus.VirtualMachine;


List<string> tests = new List<string>(){
	"auto a;a = 1+3*2;putchar(a);",
	"""
	auto a;
	a = 2;
	double(f){
	  auto c,d;
	  c = f;
	  d = f;
	  return(c+d);
	}
	a = double(1) + double(a);
	
	putint(a);
	""",
	"""
	foo(a,b){
	  auto c;
	  c = a+b;
	  auto d;
	  d = c+c;
	  return(d);
	}
	putint(foo(1,3));
	""",
	"""
	double(v) {
		return (v * 2);
	}
	
	auto a;
	a = 1;
	
	a = double(4);
	putint(a);
	""",
	"if(0){putint(1);}",
	"if(1){putint(1);}",
	"if(0){putint(1);}else{putint(2);}",
	"if(1){putint(1);}else{putint(2);}",
};

for (int i = 0; i < 1000; i++)
{
	foreach (var test in tests)
	{
		
		Parser p = new Parser(new Lexer(test));
		var tree = p.Parse();
		Compiler c = new Compiler(new VMRunner());
		c.NewCompile(tree);
		var env = c.GetEnvironment();
		VirtualMachine vm = new VirtualMachine(env);
		vm.Run();
	}
}