using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace Darkengines.Expressions.Converters {
	public class ConversionContext {
		public IDictionary<string, IDictionary<Type, IConverter>> Converters { get; }
		public Type[] ExtensionTypes { get; set; } = new Type[0];
		public IDictionary<string, Type> TypeIdentifiers { get; set; } = new Dictionary<string, Type>();
		public ConversionContext(IEnumerable<IConverter> converters) {
			Converters = converters.GroupBy(
				converter => converter.Language,
				(language, languageConverters) => new KeyValuePair<string, IDictionary<Type, IConverter>>(
					language,
					languageConverters.ToDictionary(languageConverter => languageConverter.NodeType)
				)
			).ToDictionary(languages => languages.Key, language => language.Value);
		}
		public IConverter GetSyntaxNodeConverter(string language, object node) {
			var nodeType = node.GetType();
			if (Converters.TryGetValue(language, out var converters)) {
				if (converters.TryGetValue(nodeType, out var converter)) {
					return converter;
				}
			}
			throw new NotImplementedException($"No converter found for language {language}, {nodeType.Name}.");
		}
		public ConverterResult Convert(string language, object node, ConverterScope scope, ConversionArgument? argument = null) {
			return GetSyntaxNodeConverter(language, node).Convert(node, this, scope, argument);
		}
		public Type? ResolveTypeIdentifier(string identifier) {
			if (TypeIdentifiers.TryGetValue(identifier, out var type)) {
				return type;
			}
			return null;
		}
	}
}