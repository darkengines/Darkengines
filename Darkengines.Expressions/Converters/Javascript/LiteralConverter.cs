using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.Javascript {
	public class LiteralConverter : JavascriptExpressionConverter<Esprima.Ast.Literal> {
		protected JsonSerializer JsonSerializer { get; }
		public LiteralConverter(JsonSerializer jsonSerializer) : base() {
			JsonSerializer = jsonSerializer;
		}

		public override ConverterResult Convert(Esprima.Ast.Literal syntaxNode, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? argument) {
			var value = syntaxNode.Value;
			if (argument != null) {
				using var reader = new StringReader(syntaxNode.Raw);
				using var jsonReader = new JsonTextReader(reader);
				value = JsonSerializer.Deserialize(jsonReader, argument.ExpectedType);
			}
			var expression = Expression.Constant(value);
			return new ConverterResult(expression);
		}
	}
}
