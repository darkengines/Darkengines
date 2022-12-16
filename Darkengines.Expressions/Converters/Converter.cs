using Microsoft.CodeAnalysis;

namespace Darkengines.Expressions.Converters {
	public abstract class Converter<TNode> : IConverter {
		public abstract string Language { get; }
		public Type NodeType { get { return typeof(TNode); } }
		public virtual bool IsGenericType { get { return false; } }
		ConverterResult IConverter.Convert(object node, ConversionContext converterContext, ConverterScope scope, ConversionArgument? arguments) {
			return Convert((TNode)node, converterContext, scope, arguments);
		}
		Type IConverter.GetGenericType(object node, ConversionArgument conversionArgument) { return GetGenericType((TNode)node, conversionArgument); }
		public abstract ConverterResult Convert(TNode node, ConversionContext converterContext, ConverterScope scope, ConversionArgument? arguments = null);
		public virtual Type GetGenericType(TNode syntaxNode, ConversionArgument conversionArgument) { throw new NotImplementedException(); }
	}
}