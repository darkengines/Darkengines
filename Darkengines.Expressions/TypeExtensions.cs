using Darkengines.Expressions.Converters;
using Darkengines.Expressions.Converters.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions {
	public static class TypeExtensions {

		public static string GetNameWithoutGenericArity(this Type type) {
			string name = type.Name;
			int index = name.IndexOf('`');
			return index == -1 ? name : name.Substring(0, index);
		}
		public static Type ReplaceGenericArguments(this Type type, IDictionary<Type, Type> genericArgumentMap) {
			if (type.IsGenericMethodParameter || type.IsGenericTypeParameter) {
				if (genericArgumentMap.TryGetValue(type, out var genericArgument)) {
					type = genericArgument;
				}
			}
			if (type.IsGenericType && type.ContainsGenericParameters) {
				var typeGenericArguments = type.GetGenericArguments()
				.Select(genericArgument => genericArgument.ReplaceGenericArguments(genericArgumentMap)).ToArray();
				return type.GetGenericTypeDefinition().MakeGenericType(typeGenericArguments);
			}
			return type;
		}
		public static IEnumerable<InvocationInfo> FindMethodInfos(this Type type, string name, InvocationArgumentInfo? invocationMemberArgumentInfo, InvocationArgumentInfo[] invocationArgumentInfos) {
			var invocationInfos = type.GetMethods()
				.Select(methodInfo => {
					var invocationInfo = default(InvocationInfo);
					var isCandidate = methodInfo.Name == name;
					if (isCandidate) {
						var candidateInvocationArgumentInfos = invocationArgumentInfos.AsEnumerable();
						var argumentCount = invocationArgumentInfos.Length;
						var candidateInvocationMemberArgumentInfo = invocationMemberArgumentInfo;
						var isExtensionMethod = methodInfo.GetCustomAttribute<ExtensionAttribute>() != null;
						var parameters = methodInfo.GetParameters();
						var maxArgumentCount = parameters.Length;
						var minArgumentCount = parameters.Where(p => !p.IsOptional).Count();
						if (isExtensionMethod) {
							candidateInvocationArgumentInfos = candidateInvocationArgumentInfos.Prepend(candidateInvocationMemberArgumentInfo!);
							candidateInvocationMemberArgumentInfo = null;
							argumentCount++;
						}
						isCandidate = argumentCount >= minArgumentCount && argumentCount <= maxArgumentCount;
						if (isCandidate) invocationInfo = new InvocationInfo(methodInfo, parameters, candidateInvocationMemberArgumentInfo, candidateInvocationArgumentInfos);
					}
					return invocationInfo;
				}).Where(invocationInfo => invocationInfo != null);
			return invocationInfos!;
		}
		public static InvocationInfo? FindMethodInfo(this IEnumerable<InvocationInfo> invocationInfos, ConverterScope scope) {
			var invocationInfo = invocationInfos.FirstOrDefault(invocationInfo => {
				var inferredGenericArguments = new Dictionary<Type, Type>();
				var argumentTypes = invocationInfo.ArgumentInfos.Zip(invocationInfo.ParameterInfos, (first, second) => (ArgumentInfo: first, ParameterInfo: second))
				.Select(candidateParameterArgumentTuple => {
					if (candidateParameterArgumentTuple.ArgumentInfo.ConversionResult == null) {
						var parameterType = candidateParameterArgumentTuple.ParameterInfo.ParameterType;
						var genericArguments = parameterType.GetGenericParameters().ToArray();
						if (parameterType.ContainsGenericParameters) {
							parameterType = parameterType.ReplaceGenericArguments(inferredGenericArguments);
						}
						if (typeof(Expression).IsAssignableFrom(parameterType)) parameterType = parameterType.GetGenericArguments()[0];
						//if (candidateParameterArgumentTuple.ArgumentInfo.GenericType.GetGenericParameters().Count() != parameterType.GenericTypeArguments.Length) return null;
						candidateParameterArgumentTuple.ArgumentInfo.GenericType = parameterType;
						var argumentType = candidateParameterArgumentTuple.ArgumentInfo.GenericType!.InferGenericArguments(parameterType, inferredGenericArguments);
						candidateParameterArgumentTuple.ArgumentInfo.ConversionResult = candidateParameterArgumentTuple.ArgumentInfo.Converter.Convert(
							candidateParameterArgumentTuple.ArgumentInfo.Node,
							candidateParameterArgumentTuple.ArgumentInfo.ConverterContext,
							scope,
							new ConversionArgument() {
								GenericArguments = genericArguments.Select(ga => inferredGenericArguments[ga]).ToArray(),
								ExpectedType = argumentType
							}
						);
						argumentType.InferGenericArguments(candidateParameterArgumentTuple.ArgumentInfo.ConversionResult!.Expression!.Type, inferredGenericArguments);
						if (typeof(Expression).IsAssignableFrom(argumentType)) argumentType = argumentType.GetGenericArguments()[0];
						return candidateParameterArgumentTuple.ArgumentInfo.ConversionResult.Expression.Type;
					} else {
						//candidateParameterArgumentTuple.ArgumentInfo.GenericType = parameterType;
						//var argumentType = candidateParameterArgumentTuple.ArgumentInfo.GenericType!.InferGenericArguments(parameterType, inferredGenericArguments);
						var argumentType = candidateParameterArgumentTuple.ParameterInfo.ParameterType.InferGenericArguments(candidateParameterArgumentTuple.ArgumentInfo.ConversionResult!.Expression!.Type, inferredGenericArguments);
						candidateParameterArgumentTuple.ArgumentInfo.ConversionResult = candidateParameterArgumentTuple.ArgumentInfo.Converter.Convert(
							candidateParameterArgumentTuple.ArgumentInfo.Node,
							candidateParameterArgumentTuple.ArgumentInfo.ConverterContext,
							scope,
							new ConversionArgument() {
								//GenericArguments = genericArguments.Select(ga => inferredGenericArguments[ga]).ToArray(),
								ExpectedType = argumentType
							}
						);
						if (!argumentType.IsAssignableFrom(candidateParameterArgumentTuple.ArgumentInfo.ConversionResult!.Expression!.Type)) return null;
						return argumentType;
					}
				}).ToArray();
				if (invocationInfo.MethodInfo.IsGenericMethod) {
					var genericArguments = invocationInfo.MethodInfo.GetGenericArguments().Select(genericArgument => inferredGenericArguments[genericArgument]).ToArray();
					invocationInfo.MethodInfo = invocationInfo.MethodInfo.MakeGenericMethod(genericArguments);
				}
				return argumentTypes.All(argumentType => argumentType != null);
			});
			return invocationInfo;
		}
		public static Type InferGenericArguments(this Type type, Type target, Dictionary<Type, Type> inferredTypes) {
			if (type.IsGenericMethodParameter || type.IsGenericTypeParameter) {
				inferredTypes[type] = target;
				return target;
			}
			if (type.ContainsGenericParameters) {
				var typeGenericArguments = type.GetGenericArguments();
				var targetGenericArguments = target.IsArray ? new[] { target.GetElementType()! } : target.GetGenericArguments();
				var genericArguments = typeGenericArguments.Zip(targetGenericArguments)
				.Select(tuple => tuple.First.InferGenericArguments(tuple.Second, inferredTypes)).ToArray();
				return type.GetGenericTypeDefinition().MakeGenericType(genericArguments);
			}
			return type;
		}
		public static IEnumerable<Type> GetGenericParameters(this Type type) {
			if (type.IsGenericParameter) yield return type;
			var arguments = type.GetGenericArguments();
			foreach (var argument in arguments) {
				var genericParameters = argument.GetGenericParameters();
				foreach (var genericParameter in genericParameters) yield return genericParameter;
			}
		}
		public static bool IsExtensionMethod(this MethodInfo methodInfo) {
			return methodInfo.GetCustomAttribute<ExtensionAttribute>() != null;
		}
		public static Type GetEnumerableUnderlyingType(this Type type) {
			if (type.IsArray) {
				return type.GetElementType()!;
			} else if (type.IsInterface && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
				return type.GetGenericArguments()[0];
			} else {
				return (type.IsInterface ? new[] { type } : type.GetInterfaces()).Where(@interface =>
					@interface.IsGenericType
					&& typeof(IEnumerable).IsAssignableFrom(@interface.GetGenericTypeDefinition())
				).First().GetGenericArguments()[0];
			}
		}
	}
}
