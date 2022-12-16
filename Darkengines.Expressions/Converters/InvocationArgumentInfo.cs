using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters {
	public class InvocationArgumentInfo {
		public object Node { get; }
		public IConverter Converter { get; }
		public ConverterResult? ConversionResult { get; set; }
		public ConversionContext ConverterContext { get; }
		public Type? GenericType { get; set; }
		public InvocationArgumentInfo(
			IConverter converter,
			object node,
			ConversionContext converterContext,
			ConverterScope scope,
			ConversionArgument argument
		) {
			Converter = converter;
			Node = node;
			ConverterContext = converterContext;
			if (Converter.IsGenericType) {
				GenericType = Converter.GetGenericType(Node, argument);
			} else {
				ConversionResult = Converter.Convert(node, ConverterContext, scope, argument);
			}
		}
		public override int GetHashCode() {
			if (!Converter.IsGenericType) return ConversionResult!.Expression!.Type.GetHashCode();
			return GenericType!.GetHashCode();
		}
		public override bool Equals(object? obj) {
			var result = default(bool);
			var right = obj as InvocationArgumentInfo;
			if (right != null) {
				if (!Converter.IsGenericType) {
					result = ConversionResult!.Expression!.Type == right.ConversionResult?.Expression?.Type;
				} else {
					result = GenericType == right.GenericType;
				}
			}
			return result;
		}
	}
}
