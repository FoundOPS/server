using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Telerik.Data
{
    internal class DynamicObjectBuilder
    {
        private static readonly Dictionary<TypeSignature, Type> TypesCache = new Dictionary<TypeSignature, Type>();

        private static readonly AssemblyBuilder MicroModelAssemblyBuilder =
            AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicObjects"), AssemblyBuilderAccess.Run);

        private static readonly ModuleBuilder MicroModelModuleBuilder =
            MicroModelAssemblyBuilder.DefineDynamicModule("DynamicObjectsModule", true);

        private static readonly MethodInfo GetValueMethod =
            typeof(DynamicObject).GetMethod("GetValue", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo SetValueMethod =
            typeof(DynamicObject).GetMethod("SetValue", BindingFlags.Instance | BindingFlags.NonPublic);

        public static Type GetDynamicObjectBuilderType(IEnumerable<DataColumn> properties)
        {
            Type type;
            var signature = new TypeSignature(properties);

            if (!TypesCache.TryGetValue(signature, out type))
            {
                type = CreateDynamicObjectBuilderType(properties);
                TypesCache.Add(signature, type);
            }

            return type;
        }

        private static Type CreateDynamicObjectBuilderType(IEnumerable<DataColumn> columns)
        {
            var typeBuilder =
                MicroModelModuleBuilder.DefineType("DynamicObjectBuilder_" + Guid.NewGuid(), TypeAttributes.Public, typeof(DynamicObject));

            foreach (var property in columns)
            {
                var propertyBuilder = typeBuilder.DefineProperty(property.ColumnName, PropertyAttributes.None, property.DataType, null);

                CreateGetter(typeBuilder, propertyBuilder, property);
                CreateSetter(typeBuilder, propertyBuilder, property);
            }

            return typeBuilder.CreateType();

        }

        private static void CreateGetter(TypeBuilder typeBuilder, PropertyBuilder propertyBuilder, DataColumn column)
        {
            var getMethodBuilder = typeBuilder.DefineMethod(
                "get_" + column.ColumnName,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                CallingConventions.HasThis,
                column.DataType, Type.EmptyTypes);

            var getMethodIL = getMethodBuilder.GetILGenerator();
            getMethodIL.Emit(OpCodes.Ldarg_0);
            getMethodIL.Emit(OpCodes.Ldstr, column.ColumnName);
            getMethodIL.Emit(OpCodes.Callvirt, GetValueMethod.MakeGenericMethod(column.DataType));
            getMethodIL.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethodBuilder);
        }

        private static void CreateSetter(TypeBuilder typeBuilder, PropertyBuilder propertyBuilder, DataColumn column)
        {
            var setMethodBuilder = typeBuilder.DefineMethod(
                "set_" + column.ColumnName,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                CallingConventions.HasThis,
                null, new[] { column.DataType });

            var setMethodIL = setMethodBuilder.GetILGenerator();
            setMethodIL.Emit(OpCodes.Ldarg_0);
            setMethodIL.Emit(OpCodes.Ldstr, column.ColumnName);
            setMethodIL.Emit(OpCodes.Ldarg_1);
            setMethodIL.Emit(OpCodes.Callvirt, SetValueMethod.MakeGenericMethod(column.DataType));
            setMethodIL.Emit(OpCodes.Ret);

            propertyBuilder.SetSetMethod(setMethodBuilder);
        }
    }
}
