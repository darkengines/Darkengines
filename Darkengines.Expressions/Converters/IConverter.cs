using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters {
	public interface IConverter {
		string Language { get; }
		ConverterResult Convert(object node, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? argument = null);
		bool IsGenericType { get; }
		Type NodeType { get; }

		Type GetGenericType(object node, ConversionArgument? conversionArgument = null);
	}
}
