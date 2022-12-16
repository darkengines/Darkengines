using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Darkengines.Expressions.Security {
	public class PermissionEntityTypeBuilder {
		protected IModel Model { get; }
		protected AssemblyBuilder AssemblyBuilder { get; }
		protected ModuleBuilder ModuleBuilder { get; }
		public IEnumerable<IEntityType> Types { get; set; }
		public ConcurrentDictionary<IEntityType, Type> TypeMap { get; set; }
		public ConcurrentDictionary<IPropertyBase, PropertyInfo> PropertyMap { get; set; } = new ConcurrentDictionary<IPropertyBase, PropertyInfo>();
		public ConcurrentDictionary<IPropertyBase, PropertyInfo> PermissionPropertyMap { get; set; } = new ConcurrentDictionary<IPropertyBase, PropertyInfo>();

		public PermissionEntityTypeBuilder(IModel model) {
			Model = model;
			AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName($"Darkengines.Expressions.Security.Virtual"), AssemblyBuilderAccess.Run);
			ModuleBuilder = AssemblyBuilder.DefineDynamicModule(GetType().Namespace!);
			var entityTypes = Model.GetEntityTypes();
			var cache = new Dictionary<IEntityType, TypeBuilder>();
			var permissionTypes = BuildPermissionTypes(entityTypes, cache).ToArray();
			TypeMap = new ConcurrentDictionary<IEntityType, Type>(cache.ToDictionary(pair => pair.Key, pair => pair.Value.CreateType()!));
			PropertyMap = new ConcurrentDictionary<IPropertyBase, PropertyInfo>(PropertyMap.ToDictionary(pair => pair.Key, pair => TypeMap[(IEntityType)pair.Key.DeclaringType].GetProperty(pair.Value.Name)!));
			PermissionPropertyMap = new ConcurrentDictionary<IPropertyBase, PropertyInfo>(PermissionPropertyMap.ToDictionary(pair => pair.Key, pair => TypeMap[(IEntityType)pair.Key.DeclaringType].GetProperty(pair.Value.Name)!));
			Types = TypeMap.Keys;
		}

		public IEnumerable<TypeBuilder> BuildPermissionTypes(IEnumerable<IEntityType> entityTypes, Dictionary<IEntityType, TypeBuilder> cache) {
			return entityTypes.Select(entityType => BuildPermissionType(entityType, cache));
		}

		protected TypeBuilder BuildPermissionType(IEntityType entityType, Dictionary<IEntityType, TypeBuilder> cache) {
			var dynamicTypeName = $"{entityType.Name}Permission";
			var typeBuilder = default(TypeBuilder);
			if (!cache.TryGetValue(entityType, out typeBuilder)) {
				typeBuilder = ModuleBuilder.DefineType(dynamicTypeName, TypeAttributes.Public, typeof(PermissionEntity));
				cache[entityType] = typeBuilder;
				var properties = entityType.GetProperties();
				var navigations = entityType.GetNavigations();
				foreach (var property in properties.Where(p => p.PropertyInfo != null)) {
					var propertyBuilder = CreateProperty(typeBuilder, property.Name, property.ClrType);
					PropertyMap[property] = propertyBuilder;
					var permissionPropertyInfo = CreateProperty(typeBuilder, $"{property.Name}Permission", typeof(bool));
					PermissionPropertyMap[property] = permissionPropertyInfo;
				}
				foreach (var navigation in navigations.Where(p => p.PropertyInfo != null)) {
					var permissionType = BuildPermissionType(navigation.TargetEntityType, cache);
					var propertyBuilder = CreateProperty(typeBuilder, navigation.Name, navigation.IsCollection ? permissionType.MakeArrayType() : permissionType);
					var permissionPropertyInfo = CreateProperty(typeBuilder, $"{navigation.Name}Permission", typeof(bool));
					PermissionPropertyMap[navigation] = permissionPropertyInfo;
					PropertyMap[navigation] = propertyBuilder;
				}
			}
			return typeBuilder!;
		}

		protected PropertyBuilder EmitAutoProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType) {
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

			return propertyBuilder!;
		}

		private static PropertyBuilder CreateProperty(TypeBuilder tb, string propertyName, Type propertyType) {
			FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

			PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
			MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
			ILGenerator getIl = getPropMthdBldr.GetILGenerator();

			getIl.Emit(OpCodes.Ldarg_0);
			getIl.Emit(OpCodes.Ldfld, fieldBuilder);
			getIl.Emit(OpCodes.Ret);

			MethodBuilder setPropMthdBldr =
				tb.DefineMethod("set_" + propertyName,
				  MethodAttributes.Public |
				  MethodAttributes.SpecialName |
				  MethodAttributes.HideBySig,
				  null, new[] { propertyType });

			ILGenerator setIl = setPropMthdBldr.GetILGenerator();
			Label modifyProperty = setIl.DefineLabel();
			Label exitSet = setIl.DefineLabel();

			setIl.MarkLabel(modifyProperty);
			setIl.Emit(OpCodes.Ldarg_0);
			setIl.Emit(OpCodes.Ldarg_1);
			setIl.Emit(OpCodes.Stfld, fieldBuilder);

			setIl.Emit(OpCodes.Nop);
			setIl.MarkLabel(exitSet);
			setIl.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(getPropMthdBldr);
			propertyBuilder.SetSetMethod(setPropMthdBldr);

			return propertyBuilder;
		}

	}
}
