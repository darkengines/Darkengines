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
		public bool TryGetPropertyResolver<TProperty>(Expression<Func<TItem, TProperty>> propertyExpression, object key, Expression context, Expression instanceExpression, out Expression? resolver) {
			var propertyInfo = ExpressionHelper.ExtractPropertyInfo(propertyExpression);
			return TryGetPropertyResolver(propertyInfo, key, context, instanceExpression, out resolver);
		}
		public override bool TryGetPropertyResolver(PropertyInfo propertyInfo, object key, Expression context, Expression instanceExpression, out Expression? resolver) {
			if (PropertyRuleMaps.TryGetValue(propertyInfo, out var propertyRuleMap)) {
				return propertyRuleMap.TryGetResolver(key, context, instanceExpression, out resolver);
			}
			resolver = null;
			return false;
		}
		public bool TryGetPropertyOperationResolver<TProperty>(Expression<Func<TItem, TProperty>> propertyExpression, Operation operation, Expression context, Expression instanceExpression, out Expression? resolver) {
			var propertyInfo = ExpressionHelper.ExtractPropertyInfo(propertyExpression);
			return TryGetPropertyOperationResolver(propertyInfo, operation, context, instanceExpression, out resolver);
		}
		public override bool TryGetPropertyOperationResolver(PropertyInfo propertyInfo, Operation operation, Expression context, Expression instanceExpression, out Expression? resolver) {
			return TryGetPropertyResolver(propertyInfo, operation, context, instanceExpression, out resolver);
		}
	}
}
