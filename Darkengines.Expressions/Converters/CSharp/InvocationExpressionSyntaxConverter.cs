using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.CSharp {
    public class InvocationExpressionSyntaxConverter : CSharpExpressionConverter<InvocationExpressionSyntax> {
        protected IDictionary<InvocationInfoCacheKey, MethodInfo> MethodInfoCache { get; }
        public InvocationExpressionSyntaxConverter() : base() {
            MethodInfoCache = new Dictionary<InvocationInfoCacheKey, MethodInfo>();
        }
        public override ConverterResult Convert(InvocationExpressionSyntax node, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? conversionArgument) {
            var type = default(Type);
            var memberExpressionSyntax = node.Expression as MemberAccessExpressionSyntax;
            if (memberExpressionSyntax != null) {
                var methodName = memberExpressionSyntax.Name.Identifier.ValueText;
                var memberSyntaxConverter = syntaxNodeConverterContext.GetSyntaxNodeConverter(Language, memberExpressionSyntax.Expression);
                var memberSyntaxExpressionArgumentInfo = new InvocationArgumentInfo(memberSyntaxConverter, memberExpressionSyntax.Expression, syntaxNodeConverterContext, scope, conversionArgument);
                var memberExpression = memberSyntaxExpressionArgumentInfo.ConversionResult?.Expression;
                if (memberExpression != null) {
                    type = memberExpression.Type;
                } else {
                    var typeIdentifier = memberExpressionSyntax.Expression.ToString();
                    type = syntaxNodeConverterContext.ResolveTypeIdentifier(typeIdentifier);
                    if (type == null) {
                        type = BuiltInDotNetTypeMap.GetClrType(typeIdentifier);
                    }
                }
                if (type == null) throw new InvalidOperationException($"Failed to resolve type of expression {memberExpressionSyntax.Expression}.");
                var invocationArgumentInfos = node.ArgumentList.Arguments.Select(argument => new InvocationArgumentInfo(
                    syntaxNodeConverterContext.GetSyntaxNodeConverter(Language, argument.Expression),
                    argument.Expression,
                    syntaxNodeConverterContext,
                    scope,
                    conversionArgument
                )).ToArray();
                var areArgumentClosedTypes = invocationArgumentInfos.All(invocationArgumentInfo => invocationArgumentInfo.ConversionResult?.Expression != null);
                var methodInfo = default(MethodInfo);
                var argumentExpressions = default(Expression[]);
                if (areArgumentClosedTypes) {
                    argumentExpressions = invocationArgumentInfos.Select(invocationArgumentInfo => invocationArgumentInfo.ConversionResult!.Expression!).ToArray();
                    var argumentTypes = argumentExpressions.Select(argumentExpression => argumentExpression.Type).ToArray();
                    methodInfo = type.GetMethod(methodName, argumentTypes);
                }
                if (methodInfo == null) {
                    var cacheKey = new InvocationInfoCacheKey(methodName, memberSyntaxExpressionArgumentInfo, invocationArgumentInfos);
                    if (MethodInfoCache.TryGetValue(cacheKey, out methodInfo)) {
                        if (methodInfo.IsExtensionMethod()) {
                            invocationArgumentInfos = invocationArgumentInfos.Prepend(memberSyntaxExpressionArgumentInfo).ToArray();
                            memberExpression = null;
                        }
                        argumentExpressions = methodInfo.GetParameters().Zip(invocationArgumentInfos).Select(tuple => {
                            if (tuple.Second.ConversionResult != null) return tuple.Second.ConversionResult.Expression!;
                            var inferredGenericArguments = new Dictionary<Type, Type>();
                            var argumentGenericType = tuple.Second.Converter.GetGenericType(tuple.Second.Node);
                            var argumentType = tuple.Second.Converter.GetGenericType(tuple.Second.Node).InferGenericArguments(tuple.First.ParameterType, inferredGenericArguments);
                            var genericArguments = argumentType.GetGenericArguments();
                            return tuple.Second.Converter.Convert(tuple.Second.Node, syntaxNodeConverterContext, scope, new ConversionArgument() { GenericArguments = conversionArgument.GenericArguments }).Expression!;
                        }).ToArray();
                    } else {
                        var invocationInfo = default(InvocationInfo);
                        var invocationInfos = syntaxNodeConverterContext.ExtensionTypes.Prepend(type).SelectMany(
                            type => type.FindMethodInfos(methodName, memberSyntaxExpressionArgumentInfo, invocationArgumentInfos).ToArray()
                        );
                        var inferredGenericArguments = new Dictionary<Type, Type>();
                        var tuple = invocationInfos.FindMethodInfo(scope, inferredGenericArguments);
                        invocationInfo = tuple.Value.invocationInfo;
                        if (invocationInfo != null) {
                            if (!MethodInfoCache.ContainsKey(cacheKey)) {
                                MethodInfoCache[cacheKey] = invocationInfo.MethodInfo;
                            }
                            argumentExpressions = invocationInfo.ArgumentInfos.Select(argumentInfo => argumentInfo.ConversionResult!.Expression!).ToArray();
                            memberExpression = invocationInfo.MemberInfo?.ConversionResult?.Expression;
                            methodInfo = invocationInfo.MethodInfo;
                        }
                    }
                }
                if (methodInfo == null) throw (new InvalidOperationException($"Failed to resolve MethodInfo for expression {node}."));
                var expression = Expression.Call(memberExpression, methodInfo, argumentExpressions);
                return new ConverterResult(expression);
            }
            throw new NotImplementedException();
        }
    }

    public class InvocationInfoCacheKey {
        public string MethodName { get; }
        public InvocationArgumentInfo? MemberInvocationInfo { get; }
        public InvocationArgumentInfo[] ArgumentInvocationInfos { get; }
        public InvocationInfoCacheKey(string methodName, InvocationArgumentInfo? memberInvocationInfo, InvocationArgumentInfo[] argumentInvocationInfos) {
            MethodName = methodName;
            MemberInvocationInfo = memberInvocationInfo;
            ArgumentInvocationInfos = argumentInvocationInfos;
        }
        public override int GetHashCode() {
            int argumentsHashCode = ArgumentInvocationInfos.Length;
            foreach (var argumentInvocationInfo in ArgumentInvocationInfos) {
                argumentsHashCode = unchecked(argumentsHashCode * 314159 + argumentInvocationInfo.GetHashCode());
            }
            if (MemberInvocationInfo != null) return (MethodName, MemberInvocationInfo, argumentsHashCode).GetHashCode();
            return (MethodName, argumentsHashCode).GetHashCode();
        }
        public override bool Equals(object? obj) {
            var result = default(bool);
            var invocationInfoCacheKey = obj as InvocationInfoCacheKey;
            if (invocationInfoCacheKey != null) {
                if (MemberInvocationInfo != null) {
                    result = MethodName == invocationInfoCacheKey.MethodName
                    && MemberInvocationInfo.Equals(invocationInfoCacheKey.MemberInvocationInfo)
                    && ArgumentInvocationInfos.Zip(invocationInfoCacheKey.ArgumentInvocationInfos).All(tuple => tuple.First.Equals(tuple.Second));
                } else {
                    result = MethodName == invocationInfoCacheKey.MethodName
                    && ArgumentInvocationInfos.Zip(invocationInfoCacheKey.ArgumentInvocationInfos).All(tuple => tuple.First.Equals(tuple.Second));
                }
            }
            return result;
        }
    }
}
