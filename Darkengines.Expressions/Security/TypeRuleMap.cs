using System.Linq.Expressions;
using System.Reflection;

namespace Darkengines.Expressions.Security {
	public class TypeRuleMap<TItem, TContext> : RuleMap<TItem, TContext> {
		protected IDictionary<PropertyInfo, IPropertyRuleMap> PropertyRuleMaps { get; }
		public TypeRuleMap() : base() {
			PropertyRuleMaps = new Dictionary<PropertyInfo, IPropertyRuleMap>();
		}
		public PropertyRuleMap<TItem, TProperty, TContext> Expose<TProperty>(Expression<Func<TItem, TProperty>> propertyExpression) {
			var propertyInfo = ExpressionHelper.ExtractPropertyInfo(propertyExpression);
			if (!PropertyRuleMaps.TryGetValue(propertyInfo, out var propertyRuleMap)) {
				PropertyRuleMaps[propertyInfo] = propertyRuleMap = new PropertyRuleMap<TItem, TProperty, TContext>(propertyInfo);
			}
			return (PropertyRuleMap<TItem, TProperty, TContext>)propertyRuleMap;
		}
		public Expression? GetPropertyResolver<TProperty>(Expression<Func<TItem, TProperty>> propertyExpression, object key, Expression context, Expression instanceExpression) {
			var propertyInfo = ExpressionHelper.ExtractPropertyInfo(propertyExpression);
			return GetPropertyResolver(propertyInfo, key, context, instanceExpression);
		}
		public override Expression? GetPropertyResolver(PropertyInfo propertyInfo, object key, Expression context, Expression instanceExpression) {
			var resolver = default(Expression?);
			if (PropertyRuleMaps.TryGetValue(propertyInfo, out var propertyRuleMap)) {
				resolver = propertyRuleMap.GetResolver(key, context, instanceExpression);
			} else {
				throw new KeyNotFoundException($"No resolver found for property {propertyInfo.Name} of type {propertyInfo.DeclaringType.Name} with key {key}.");
			}
			return resolver;
		}
		public Expression? GetPropertyOperationResolver<TProperty>(Expression<Func<TItem, TProperty>> propertyExpression, Operation operation, Expression context, Expression instanceExpression) {
			var propertyInfo = ExpressionHelper.ExtractPropertyInfo(propertyExpression);
			return GetPropertyOperationResolver(propertyInfo, operation, context, instanceExpression);
		}
		public override Expression? GetPropertyOperationResolver(PropertyInfo propertyInfo, Operation operation, Expression context, Expression instanceExpression) {
			return GetPropertyResolver(propertyInfo, operation, context, instanceExpression);
		}
	}
}
