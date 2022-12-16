// See https://aka.ms/new-console-template for more information
using Antlr4.Runtime;
using Darkengines.AntlrTester;

Console.WriteLine("Hello, World!");

var code = @"user.filter(u => u.id % 2 == 0)";
var stream = new Antlr4.Runtime.AntlrInputStream(code);
using (var output = new StringWriter()) {
	using (var errorOutput = new StringWriter()) {
		var lexer = new TypeScriptLexer(stream, output, errorOutput);
		var commonTokenStream = new CommonTokenStream(lexer);
		var parser = new TypeScriptParser(commonTokenStream);
		var x = parser.program();
	}
}