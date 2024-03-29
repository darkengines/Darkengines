﻿using Darkengines.Expressions.Converters;
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
                            var instanceType = parameters.First().ParameterType;
                            isCandidate &= instanceType.GenericAssignableFrom(invocationMemberArgumentInfo.ConversionResult.Expression.Type!);
                            candidateInvocationArgumentInfos = candidateInvocationArgumentInfos.Prepend(candidateInvocationMemberArgumentInfo!);
                            candidateInvocationMemberArgumentInfo = null;
                            argumentCount++;
                        }
                        isCandidate &= argumentCount >= minArgumentCount && argumentCount <= maxArgumentCount;
                        if (isCandidate) invocationInfo = new InvocationInfo(methodInfo, parameters, candidateInvocationMemberArgumentInfo, candidateInvocationArgumentInfos);
                    }
                    return invocationInfo;
                }).Where(invocationInfo => invocationInfo != null);
            return invocationInfos!;
        }
        public static (InvocationInfo invocationInfo, IDictionary<Type, Type> inferredGenericArguments)? FindMethodInfo(this IEnumerable<InvocationInfo> invocationInfos, ConverterScope scope, Dictionary<Type, Type> inferredGenericArguments) {
            var invocationInfo = invocationInfos
                .Select(invocationInfo => (invocationInfo, inferredGenericArguments: new Dictionary<Type, Type>()))
                .FirstOrDefault(tuple => {
                var argumentTypes = tuple.invocationInfo.ArgumentInfos.Zip(tuple.invocationInfo.ParameterInfos, (first, second) => (ArgumentInfo: first, ParameterInfo: second))
                .Select(candidateParameterArgumentTuple => {
                    if (candidateParameterArgumentTuple.ArgumentInfo.ConversionResult == null) {
                        var parameterType = candidateParameterArgumentTuple.ParameterInfo.ParameterType;
                        var genericArguments = parameterType.GetGenericParameters().ToArray();
                        if (parameterType.ContainsGenericParameters) {
                            parameterType = parameterType.ReplaceGenericArguments(inferredGenericArguments);
                        }
                        if (typeof(Expression).IsAssignableFrom(parameterType)) parameterType = parameterType.GetGenericArguments()[0];
                        //if (candidateParameterArgumentTuple.ArgumentInfo.GenericType.GetGenericParameters().Count() != parameterType.GenericTypeArguments.Length) return null;
                        if (candidateParameterArgumentTuple.ArgumentInfo.GenericType.GenericAssignableFrom(parameterType)) {
                            candidateParameterArgumentTuple.ArgumentInfo.GenericType = parameterType;
                        }
                        var argumentType = candidateParameterArgumentTuple.ArgumentInfo.GenericType!.InferGenericArguments(parameterType, inferredGenericArguments);
                        if (argumentType == null) return null;
                        candidateParameterArgumentTuple.ArgumentInfo.ConversionResult = candidateParameterArgumentTuple.ArgumentInfo.Converter.Convert(
                            candidateParameterArgumentTuple.ArgumentInfo.Node,
                            candidateParameterArgumentTuple.ArgumentInfo.ConverterContext,
                            scope,
                            new ConversionArgument() {
                                GenericArguments = genericArguments.Select(ga => {
                                    if (inferredGenericArguments.TryGetValue(ga, out var argument)) return argument;
                                    return ga;
                                }).ToArray(),
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
                        if (argumentType == null) return null;
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
                if (tuple.invocationInfo.MethodInfo.IsGenericMethod) {
                    var genericArguments = tuple.invocationInfo.MethodInfo.GetGenericArguments().Select(genericArgument => inferredGenericArguments[genericArgument]).ToArray();
                    tuple.invocationInfo.MethodInfo = tuple.invocationInfo.MethodInfo.MakeGenericMethod(genericArguments);
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
                if (type.IsInterface) {
                    target = ((TypeInfo)target).ImplementedInterfaces.FirstOrDefault(@interface => {
                        return @interface.IsGenericType && @interface.GetGenericTypeDefinition() == type.GetGenericTypeDefinition();
                    }, null) ?? target;
                }

                var typeGenericArguments = type.GetGenericArguments();
                var targetGenericArguments = target.IsArray ? new[] { target.GetElementType()! } : target.GetGenericArguments();
                if (typeGenericArguments.Length != targetGenericArguments.Length) return null;
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
        public static bool GenericAssignableFrom(this Type to, Type from, Dictionary<Type, Type> genericArgumentMapping = null) {
            if (to.ContainsGenericParameters) {
                if (!from.ContainsGenericParameters) {
                    if (to.IsGenericParameter) {
                        var constraints = to.GetGenericParameterConstraints();
                        var isAssignable = constraints.All(constraint => constraint.IsAssignableFrom(from));
                        if (genericArgumentMapping != null && isAssignable) {
                            genericArgumentMapping[to] = from;
                        }
                        return isAssignable;
                    }
                    var toGenericTypeDefinition = to.GetGenericTypeDefinition();
                    var fromGenericTypeDefinition = from.IsGenericType ? from.GetGenericTypeDefinition() : from;
                    if (toGenericTypeDefinition.BaseType == typeof(LambdaExpression)) toGenericTypeDefinition = toGenericTypeDefinition.BaseType;
                    if (fromGenericTypeDefinition.BaseType == typeof(LambdaExpression)) toGenericTypeDefinition = fromGenericTypeDefinition.BaseType;
                    if (toGenericTypeDefinition.IsAssignableFrom(fromGenericTypeDefinition) || fromGenericTypeDefinition.GetInterfaces().Any(@interface => @interface.IsGenericType && toGenericTypeDefinition.IsAssignableFrom(@interface.GetGenericTypeDefinition()))) {
                        var fromGenericArguments = from.HasElementType ? new[] { from.GetElementType() } : from.GenericTypeArguments;
                        var toGenericArguments = to.GenericTypeArguments;
                        return fromGenericArguments.Zip(toGenericArguments, (fromGenericArgument, toGenericArgument) => (fromGenericArgument, toGenericArgument)).All(tuple => tuple.toGenericArgument.GenericAssignableFrom(tuple.fromGenericArgument, genericArgumentMapping));
                    }
                }
            } else {
                return to.IsAssignableFrom(from) || from.GetInterfaces().Any(@interface => to.IsAssignableFrom(@interface));
            }
            return false;
        }
    }
}
