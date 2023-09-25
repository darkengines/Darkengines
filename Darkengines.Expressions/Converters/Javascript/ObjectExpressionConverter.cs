using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.Javascript {
	public class ObjectExpressionConverter : JavascriptExpressionConverter<Esprima.Ast.ObjectExpression> {
		protected JsonSerializer JsonSerializer { get; }
		protected AnonymousTypeBuilder AnonymousTypeBuilder { get; }
		protected Dictionary<HashSet<Tuple<Type, string>>, Type> Cache { get; }
		public ObjectExpressionConverter(JsonSerializer jsonSerializer, AnonymousTypeBuilder anonymousTypeBuilder) : base() {
			JsonSerializer = jsonSerializer;
			AnonymousTypeBuilder = anonymousTypeBuilder;
			var comparer = HashSet<Tuple<Type, string>>.CreateSetComparer();
			Cache = new Dictionary<HashSet<Tuple<Type, string>>, Type>(comparer);
		}
		public override ConverterResult Convert(Esprima.Ast.ObjectExpression syntaxNode, ConversionContext converterContext, ConverterScope scope, ConversionArgument? argument) {
			if (argument?.ExpectedType != null) {
				var code = syntaxNode.Location.Source.Substring(syntaxNode.Range.Start, syntaxNode.Range.End - syntaxNode.Range.Start + 1);
				using (var reader = new StringReader(code)) {
					var @object = JsonSerializer.Deserialize(reader, argument.ExpectedType);
					return new ConverterResult(Expression.Constant(@object));
				}
			}
			var propertyValueExpressions = syntaxNode.Properties.Select(property => new {
				Property = property,
				ValueExpression = converterContext.Convert(Language, ((Esprima.Ast.Property)property).Value, scope)
			}).ToArray();
			var tuples = propertyValueExpressions.Select(propertyValueExpression => new Tuple<Type, string>(propertyValueExpression.ValueExpression.Expression!.Type, ((Esprima.Ast.Literal)((Esprima.Ast.Property)propertyValueExpression.Property).Key).StringValue!)).ToArray();
			var set = new HashSet<Tuple<Type, string>>(tuples);
			if (!Cache.TryGetValue(set, out var anonymousType)) {
				anonymousType = AnonymousTypeBuilder.BuildAnonymousType(set);
				Cache[set] = anonymousType;
			}
			var newExpression = Expression.New(anonymousType.GetConstructor(new Type[0])!);
			var initializationExpression = Expression.MemberInit(
				newExpression,
				propertyValueExpressions.Select(pve => Expression.Bind(anonymousType.GetProperty(((Esprima.Ast.Literal)((Esprima.Ast.Property)pve.Property).Key).StringValue!)!, pve.ValueExpression.Expression!))
			);
			return new ConverterResult(initializationExpression);
		}
	}
}
