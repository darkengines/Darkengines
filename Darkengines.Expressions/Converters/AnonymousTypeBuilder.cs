using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Darkengines.Expressions.Converters {
	public class AnonymousTypeBuilder {
		protected AssemblyBuilder AssemblyBuilder { get; }
		protected ModuleBuilder ModuleBuilder { get; }
		public Assembly Assembly { get { return AssemblyBuilder; } }
		protected IDictionary<HashSet<Tuple<Type, string>>, Type> Cache { get; } = new Dictionary<HashSet<Tuple<Type, string>>, Type>(HashSet<Tuple<Type, string>>.CreateSetComparer());

		public AnonymousTypeBuilder(string assemblyName, string moduleName) {
			AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
			ModuleBuilder = AssemblyBuilder.DefineDynamicModule(moduleName);
		}
		public Type BuildAnonymousType(HashSet<Tuple<Type, string>> propertySet, string typeName = null) {
			Type anonymousType;
			if (Cache.TryGetValue(propertySet, out anonymousType)) return anonymousType;
			var dynamicTypeName = typeName ?? Guid.NewGuid().ToString();
			var typeBuilder = ModuleBuilder.DefineType(dynamicTypeName, TypeAttributes.Public);
			var result = propertySet.Select(tuple => {
				var propertyResult = EmitAutoProperty(typeBuilder, tuple.Item2, tuple.Item1);
				return (propertyType: tuple.Item1, propertyName: tuple.Item2, propertyBuilder: propertyResult.Item2, fieldBuilder: propertyResult.Item1);
			}).ToArray();
			EmitConstructor(typeBuilder, result);
			var dynamicType = typeBuilder.CreateType();
			Cache[propertySet] = dynamicType;
			return dynamicType;
		}

        private void EmitConstructor(TypeBuilder typeBuilder, (Type propertyType, string propertyName, PropertyBuilder propertyBuilder, FieldBuilder fieldBuilder)[] properties) {
            var collectionGenericConstructor = typeof(ICollection<>).GetConstructor(Array.Empty<Type>())!;
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, Array.Empty<Type>());
            var generator = constructorBuilder.GetILGenerator();
            foreach (var property in properties.Where(property => property.propertyType != typeof(string) && typeof(ICollection).IsAssignableFrom(property.propertyType))) {
                var underlyingPropertyType = property.Item1.GetEnumerableUnderlyingType();
                var collectionConstructor = TypeBuilder.GetConstructor(underlyingPropertyType, collectionGenericConstructor);
                generator.Emit(OpCodes.Newobj, collectionConstructor);
				generator.Emit(OpCodes.Stfld, property.fieldBuilder);
            };
			generator.Emit(OpCodes.Ret);
        }

        protected (FieldBuilder, PropertyBuilder) EmitAutoProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType) {
			if (typeof(string) != propertyType && typeof(IEnumerable).IsAssignableFrom(propertyType)) {
				propertyType = typeof(IEnumerable<>).MakeGenericType(propertyType.GetEnumerableUnderlyingType());
			}
			var backingField = typeBuilder.DefineField($"<{propertyName}>k__BackingField", propertyType, FieldAttributes.Private);
			var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

			var getMethod = typeBuilder.DefineMethod("get_" + propertyName,
				MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
				propertyType,
				Type.EmptyTypes
			);
			var getGenerator = getMethod.GetILGenerator();
			getGenerator.Emit(OpCodes.Ldarg_0);
			getGenerator.Emit(OpCodes.Ldfld, backingField);
			getGenerator.Emit(OpCodes.Ret);
			propertyBuilder.SetGetMethod(getMethod);

			var setMethod = typeBuilder.DefineMethod("set_" + propertyName,
				MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
				null,
				new[] { propertyType }
			);
			var setGenerator = setMethod.GetILGenerator();
			setGenerator.Emit(OpCodes.Ldarg_0);
			setGenerator.Emit(OpCodes.Ldarg_1);
			setGenerator.Emit(OpCodes.Stfld, backingField);
			setGenerator.Emit(OpCodes.Ret);
			propertyBuilder.SetSetMethod(setMethod);

			return (backingField, propertyBuilder);
		}
	}
}
