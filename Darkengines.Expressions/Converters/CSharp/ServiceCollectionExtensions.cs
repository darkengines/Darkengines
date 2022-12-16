using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.CSharp {
	public static class ServiceCollectionExtensions {
		public static IServiceCollection AddCSharpConverters(this IServiceCollection serviceCollection) {
			return serviceCollection
				.AddSingleton<IConverter, BinaryExpressionSyntaxConverter>()
				.AddSingleton<IConverter, BlockExpressionSyntaxConverter>()
				.AddSingleton<IConverter, CompilationUnitSyntaxConverter>()
				.AddSingleton<IConverter, ExpressionStatementSyntaxConverter>()
				.AddSingleton<IConverter, GlobalStatementSyntaxConverter>()
				.AddSingleton<IConverter, IdentifierNameSyntaxConverter>()
				.AddSingleton<IConverter, IncompleteMemberSyntaxConverter>()
				.AddSingleton<IConverter, InvocationExpressionSyntaxConverter>()
				.AddSingleton<IConverter, LiteralExpressionSyntaxConverter>()
				.AddSingleton<IConverter, MemberAccessExpressionSyntaxConverter>()
				.AddSingleton<IConverter, ParameterSyntaxConverter>()
				.AddSingleton<IConverter, ParenthesizedLambdaExpressionSyntaxConverter>()
				.AddSingleton<IConverter, SimpleLambdaExpressionSyntaxConverter>();
		}
	}
}
