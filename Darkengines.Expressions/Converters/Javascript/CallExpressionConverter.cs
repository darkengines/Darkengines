using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.Javascript {
    public class CallExpressionConverter : JavascriptExpressionConverter<Esprima.Ast.CallExpression> {
        protected IDictionary<InvocationInfoCacheKey, MethodInfo> MethodInfoCache { get; }
        public CallExpressionConverter() : base() {
            MethodInfoCache = new Dictionary<InvocationInfoCacheKey, MethodInfo>();
        }
        public override ConverterResult Convert(Esprima.Ast.CallExpression node, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? conversionArgument) {
            var type = default(Type);
            if (node.Callee is Esprima.Ast.MemberExpression javascriptMemberExpression) {
                if (javascriptMemberExpression.Property is Esprima.Ast.Identifier javascriptMemberIdentifier) {
                    var methodName = javascriptMemberIdentifier.Name.ToPascalCase();
                    var memberSyntaxConverter = syntaxNodeConverterContext.GetSyntaxNodeConverter(Language, javascriptMemberExpression.Object);
                    var memberSyntaxExpressionArgumentInfo = new InvocationArgumentInfo(memberSyntaxConverter, javascriptMemberExpression.Object, syntaxNodeConverterContext, scope, conversionArgument);
                    var memberExpression = memberSyntaxExpressionArgumentInfo.ConversionResult?.Expression;
                    if (memberExpression != null) {
                        type = memberExpression.Type;
                    } else {
                        var typeIdentifier = javascriptMemberExpression.Object.ToString();
                        type = syntaxNodeConverterContext.ResolveTypeIdentifier(typeIdentifier);
                        if (type == null) {
                            type = BuiltInDotNetTypeMap.GetClrType(typeIdentifier);
                        }
                    }
                    if (type == null) throw new InvalidOperationException($"Failed to resolve type of expression {javascriptMemberExpression.Object.Location.Source}.");
                    var methodInfo = default(MethodInfo);
                    var invocationArgumentInfos = default(InvocationArgumentInfo[]);
                    var argumentExpressions = default(Expression[]);

                    var methodInfos = type.GetMethods().Where(method => method.Name == methodName && !method.ContainsGenericParameters && method.GetParameters().Length == node.Arguments.Count).ToArray();
                    if (methodInfos.Any()) {
                        methodInfo = methodInfos.First();
                        var parameterInfos = methodInfo.GetParameters();
                        invocationArgumentInfos =
                            node.Arguments.Zip(parameterInfos).Select((tuple) => new InvocationArgumentInfo(
                            syntaxNodeConverterContext.GetSyntaxNodeConverter(Language, tuple.First),
                            tuple.First,
                            syntaxNodeConverterContext,
                            scope,
                            new ConversionArgument { ExpectedType = tuple.Second.ParameterType }
                        )).ToArray();
                        argumentExpressions = invocationArgumentInfos.Select(invocationArgumentInfo => invocationArgumentInfo.ConversionResult!.Expression!).ToArray();
                    } else {
                        invocationArgumentInfos =
                            node.Arguments.Select((argument) => new InvocationArgumentInfo(
                            syntaxNodeConverterContext.GetSyntaxNodeConverter(Language, argument),
                            argument,
                            syntaxNodeConverterContext,
                            scope,
                            new ConversionArgument { }
                        )).ToArray();
                        var areArgumentClosedTypes = invocationArgumentInfos.All(invocationArgumentInfo => invocationArgumentInfo.ConversionResult?.Expression != null);

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
                                    var argumentExpression = default(Expression);
                                    if (tuple.Second.ConversionResult != null) {
                                        argumentExpression = tuple.Second.ConversionResult.Expression!;
                                    } else {
                                        var inferredGenericArguments = new Dictionary<Type, Type>();
                                        var argumentGenericType = tuple.Second.Converter.GetGenericType(tuple.Second.Node, new ConversionArgument() { ExpectedType = tuple.First.ParameterType });
                                        var argumentType = tuple.Second.Converter.GetGenericType(tuple.Second.Node, new ConversionArgument() { ExpectedType = tuple.First.ParameterType }).InferGenericArguments(tuple.First.ParameterType, inferredGenericArguments);
                                        var genericArguments = argumentType.GetGenericArguments();
                                        argumentExpression = tuple.Second.Converter.Convert(tuple.Second.Node, syntaxNodeConverterContext, scope, conversionArgument).Expression!;
                                    }

                                    var parameterType = tuple.First.ParameterType;
                                    if (argumentExpression.Type != parameterType && !parameterType.ContainsGenericParameters) {
                                        argumentExpression = Expression.Convert(argumentExpression, parameterType);
                                    }

                                    return argumentExpression;
                                }).ToArray();
                            } else {
                                var invocationInfo = default(InvocationInfo);
                                var invocationInfos = syntaxNodeConverterContext.ExtensionTypes.Prepend(type).SelectMany(
                                    type => type.FindMethodInfos(methodName, memberSyntaxExpressionArgumentInfo, invocationArgumentInfos).ToArray()
                                );
                                invocationInfo = invocationInfos.FindMethodInfo(scope);
                                if (invocationInfo != null) {
                                    if (false && !MethodInfoCache.ContainsKey(cacheKey)) {
                                        MethodInfoCache[cacheKey] = invocationInfo.MethodInfo;
                                    }
                                    argumentExpressions = invocationInfo.ArgumentInfos.Select((argumentInfo, argumentIndex) => {
                                        var parameterType = invocationInfo.ParameterInfos[argumentIndex].ParameterType;
                                        if (argumentInfo.ConversionResult!.Expression!.Type != parameterType && !parameterType.ContainsGenericParameters) {
                                            return Expression.Convert(argumentInfo.ConversionResult!.Expression, parameterType);
                                        }
                                        return argumentInfo.ConversionResult!.Expression!;
                                    }).ToArray();
                                    memberExpression = invocationInfo.MemberInfo?.ConversionResult?.Expression;
                                    methodInfo = invocationInfo.MethodInfo;
                                }
                            }
                        }
                    }
                    if (methodInfo == null) throw (new InvalidOperationException($"Failed to resolve MethodInfo for expression {node.Location.Source}."));
                    var isAsync = methodInfo.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
                    var expression = Expression.Call(memberExpression, methodInfo, argumentExpressions);
                    return new ConverterResult(expression, isAsync);
                }
            } else {
                if (node.Callee is Esprima.Ast.Identifier calleeIdentifier) {
                    if (calleeIdentifier.Name == "cast") {
                        var typeNameArgument = node.Arguments[0];
                        if (typeNameArgument is Esprima.Ast.Literal typeNameLiteral) {
                            var typeName = typeNameLiteral.StringValue;
                            var valueExpression = syntaxNodeConverterContext.Convert(Language, node.Arguments[1], scope, conversionArgument);
                            var castType = syntaxNodeConverterContext.ResolveTypeIdentifier(typeName) ?? Type.GetType(typeName);
                            var result = Expression.Convert(valueExpression.Expression, castType);
                            return new ConverterResult(result);
                        }
                    }
                }
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
