using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ServiceStack.OrmLite.zly
{
    internal static class ProxyInstance
    {
        private static Dictionary<Type, Type> typeMap = new Dictionary<Type, Type>();
        internal static T CreateInstance<T>() 
        {
            Type proxyType;
            Type type = typeof(T);
            if (typeMap.TryGetValue(type, out proxyType))
                return (T)ReflectionExtensions.CreateInstance(proxyType);

            string nameOfAssembly = type.Name + "ProxyAssembly";
            string nameOfModule = type.Name + "ProxyModule";
            string nameOfType = type.Name + "Proxy";

            var assemblyName = new AssemblyName(nameOfAssembly);
            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assembly.DefineDynamicModule(nameOfModule);

            var typeBuilder = moduleBuilder.DefineType(nameOfType, TypeAttributes.Public, typeof(T));

            InjectInterceptor<T>(typeBuilder);

            proxyType = typeBuilder.CreateType();
            typeMap.Add(type, proxyType);
            return (T)ReflectionExtensions.CreateInstance(proxyType);
        }
        private static void InjectInterceptor<T>(TypeBuilder typeBuilder)
        {
            ////获取所有有TableReferenceAttribute的属性
            //var modelDefinitions = typeof(T).GetModelDefinition();
            //var tableReferenceAttrs = modelDefinitions.IgnoredFieldDefinitions.Where(o => o.TableReferenceAttr != null);
            //if (tableReferenceAttrs == null || tableReferenceAttrs.Count() == 0)
            //{
            //    return;
            //}

            // ---- define fields ----
            var fieldInterceptor = typeBuilder.DefineField(
              "_interceptor", typeof(Interceptor), FieldAttributes.Private);

            // ---- define costructors ----

            var constructorBuilder = typeBuilder.DefineConstructor(
              MethodAttributes.Public, CallingConventions.Standard, null);
            var ilOfCtor = constructorBuilder.GetILGenerator();

            ilOfCtor.Emit(OpCodes.Ldarg_0);
            ilOfCtor.Emit(OpCodes.Newobj, typeof(Interceptor).GetConstructor(new Type[0]));
            ilOfCtor.Emit(OpCodes.Stfld, fieldInterceptor);
            ilOfCtor.Emit(OpCodes.Ret);

            // ---- define methods ----
            var methodsOfType = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            for (var i = 0; i < methodsOfType.Length; i++)
            {
                var method = methodsOfType[i];
                if (!method.IsVirtual || (!method.Name.StartsWith("get_") && !method.Name.StartsWith("set_"))||!method.IsSpecialName)
                    continue;
                var methodParameterTypes =
                  method.GetParameters().Select(p => p.ParameterType).ToArray();

                var methodBuilder = typeBuilder.DefineMethod(
                  method.Name,
                  MethodAttributes.Public | MethodAttributes.Virtual,
                  CallingConventions.Standard,
                  method.ReturnType,
                  methodParameterTypes);

                var ilOfMethod = methodBuilder.GetILGenerator();
                ilOfMethod.Emit(OpCodes.Ldarg_0);
                ilOfMethod.Emit(OpCodes.Ldfld, fieldInterceptor);

                // create instance of T
                ilOfMethod.Emit(OpCodes.Newobj, typeof(T).GetConstructor(new Type[0]));
                ilOfMethod.Emit(OpCodes.Ldstr, method.Name);

                // build the method parameters
                if (methodParameterTypes == null)
                {
                    ilOfMethod.Emit(OpCodes.Ldnull);
                }
                else
                {
                    var parameters = ilOfMethod.DeclareLocal(typeof(object[]));
                    ilOfMethod.Emit(OpCodes.Ldc_I4, methodParameterTypes.Length);
                    ilOfMethod.Emit(OpCodes.Newarr, typeof(object));
                    ilOfMethod.Emit(OpCodes.Stloc, parameters);

                    for (var j = 0; j < methodParameterTypes.Length; j++)
                    {
                        ilOfMethod.Emit(OpCodes.Ldloc, parameters);
                        ilOfMethod.Emit(OpCodes.Ldc_I4, j);
                        ilOfMethod.Emit(OpCodes.Ldarg, j + 1);
                        ilOfMethod.Emit(OpCodes.Stelem_Ref);
                    }
                    ilOfMethod.Emit(OpCodes.Ldloc, parameters);
                }

                // call Invoke() method of Interceptor
                ilOfMethod.Emit(OpCodes.Callvirt, typeof(Interceptor).GetMethod("Invoke"));

                // pop the stack if return void
                if (method.ReturnType == typeof(void))
                {
                    ilOfMethod.Emit(OpCodes.Pop);
                }

                // complete
                ilOfMethod.Emit(OpCodes.Ret);
            }
        }
    }
}
