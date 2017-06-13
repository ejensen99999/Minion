using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Minion.Inject.Aspects;

namespace Minion.Inject.Emit
{
    public class AspectEmitter
    {
        private const string MODULE_NAME = "Minion.Inject.Aspects.Types.dll";
        private const string DECORATOR = "Proxy";
        private readonly ModuleBuilder _modBuilder;

        public AspectEmitter()
        {
            var assemblyName = new AssemblyName() { Name = "Minion.Inject.Aspects.Proxies" };
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            _modBuilder = assemblyBuilder.DefineDynamicModule(MODULE_NAME);
        }

        public Type CreateAspectProxyType<T>(ConstructorInfo ctor = null)
          where T : class
        {
            return CreateAspectProxyType(typeof(T), ctor);
        }

        public Type CreateAspectProxyType(Type baseType, ConstructorInfo ctor = null)
        {
            var baseInfo = baseType.GetTypeInfo();

            if (baseInfo.IsInterface)
            {
                throw new Exception("For aspect interception the type must be an implementation");
            }

            var typeAttr = TypeAttributes.Class | TypeAttributes.Public;
            var typeBuilder = _modBuilder.DefineType(baseType.FullName + DECORATOR, typeAttr, baseType);

            if (ctor == null)
            {
                GenerateProxyConstructors(baseType, typeBuilder);
            }
            else
            {
                GenerateProxyConstructor(typeBuilder, ctor);
            }

            GenerateProxyProperties(baseType, typeBuilder);
            GenerateProxyMethods(baseType, typeBuilder);

            var targetType = typeBuilder.CreateTypeInfo();

            return targetType.AsType();

        }

        // Creates the targeted contructor for the IOC proxy
        private void GenerateProxyConstructor(TypeBuilder builder,
             ConstructorInfo ctor)
        {
            var parameters = ctor.GetParameters();
            var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();

            var ctorBuilder = builder.DefineConstructor(ctor.Attributes, ctor.CallingConvention, parameterTypes);
            var ctorIl = ctorBuilder.GetILGenerator();
            var paramCount = parameters.Length;

            ctorIl.Emit(OpCodes.Ldarg_0);

            for (var i = 1; i <= paramCount; ++i)
            {
                ctorIl.Emit(OpCodes.Ldarg, i);
            }

            ctorIl.Emit(OpCodes.Call, ctor);
            ctorIl.Emit(OpCodes.Ret);
        }

        // Creates the constructors for the proxy
        private void GenerateProxyConstructors(Type baseType, TypeBuilder builder)
        {
            foreach (var constructor in baseType.GetConstructors(BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                GenerateProxyConstructor(builder, constructor);
            }
        }

        // Creates the property overrides for the proxy
        private void GenerateProxyProperties(Type baseType, TypeBuilder typeBuilder, IEnumerable<PropertyInfo> properties = null)
        {
            var getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final;

            if (properties == null)
                properties = baseType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            foreach (var prop in properties)
            {
                var attributes = AspectExtensions.GetAttributeContainers(prop.GetCustomAttributes(true), prop.GetCustomAttributeData()).ToArray();

                if (attributes.Length == 0)
                {
                    continue;
                }

                var propBuilder = typeBuilder.DefineProperty(prop.Name, prop.Attributes, prop.PropertyType, null);
                var getname = "get_" + prop.Name;
                var setname = "set_" + prop.Name;
                var accessors = prop.GetAccessors(true);
                var accessorNames = accessors.Select(x => x.Name);


                if (accessorNames.Contains(getname))
                {
                    MethodBuilder getMBuilder = typeBuilder.DefineMethod(getname, getSetAttr, prop.PropertyType, Type.EmptyTypes);
                    ILGenerator getIl = getMBuilder.GetILGenerator();

                    //Getter interception
                    EmitOnPropertyGetting(getIl, baseType, prop.PropertyType, getname, attributes);

                    propBuilder.SetGetMethod(getMBuilder);
                }

                if (accessorNames.Contains(setname))
                {
                    MethodBuilder setMBuilder = typeBuilder.DefineMethod(setname, getSetAttr, null, new Type[] { prop.PropertyType });
                    setMBuilder.DefineParameter(1, ParameterAttributes.None, "value");
                    ILGenerator setIl = setMBuilder.GetILGenerator();

                    //Setter interception
                    var cpi = new CustomParameterInfo { ParameterType = prop.PropertyType };
                    EmitOnPropertySetting(setIl, baseType, prop.PropertyType, setname, attributes, new CustomParameterInfo[] { cpi });

                    propBuilder.SetSetMethod(setMBuilder);
                }
            }
        }

        // Creates the method overrides for the proxy
        private void GenerateProxyMethods(Type baseType, TypeBuilder builder)
        {
            var methods = baseType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var method in methods)
            {
                var attributes = AspectExtensions.GetAttributeContainers(method.GetCustomAttributes(true), method.GetCustomAttributeData()).ToArray();

                if (attributes.Length == 0
                    || method.Name.Contains("get_")
                    || method.Name.Contains("set_")
                    || method.Module.Name == "mscorlib.dll")
                {
                    continue;
                }

                var methodAttributes = method.Attributes ^ MethodAttributes.VtableLayoutMask;
                var cpi = method.GetParameters().ToCustomParameters();
                var parameterTypes = cpi.Select(x => x.ParameterType).ToArray();

                var methodBuilder = builder.DefineMethod(method.Name, methodAttributes, CallingConventions.HasThis, method.ReturnType, parameterTypes);
                var methodIl = methodBuilder.GetILGenerator();

                EmitOnMethodExecution(methodIl, baseType, method, attributes, cpi);
            }
        }

        // Passes values defined in the attribute (aspect) constructor. Must be base types defined at compile time
        private void EmitAspectObjects(ILGenerator emitIl, CustomAttributeContainer[] attributes)
        {
            var len = attributes.Count();

            if (len == 0)
            {
                return;
            }

            for (var i = 0; i < len; i++)
            {
                var attribute = attributes[i].Attribute;
                var typeAttr = attribute.GetType();
                var attr = attributes[i].Data;
                var args = attr.ConstructorArguments;

                emitIl.DeclareLocal(typeAttr);

                if (args.Count > 0)
                {
                    foreach (var arg in args)
                    {
                        emitIl.LoadForValueType(arg.Value);
                    }
                }

                emitIl.Emit(OpCodes.Newobj, attr.Constructor);
                emitIl.Emit(OpCodes.Stloc_S, i);
            }
        }

        // Creates injection of event parameters into each aspect event
        private void EmitEventParameters(ILGenerator emitIl, string name, LocalBuilder context)
        {
            emitIl.Emit(OpCodes.Ldarg_0);
            emitIl.Emit(OpCodes.Ldstr, name);
            emitIl.EmitCall(OpCodes.Call, typeof(AspectExtensions).GetMethod("CreateAspectContext", BindingFlags.Public | BindingFlags.Static), new Type[] { typeof(object), typeof(string) });
            emitIl.Emit(OpCodes.Stloc, context);
        }
        
        // Creates block wrapping the execution and return events
        private void EmitOnMethodExecution(ILGenerator emitIl, Type baseType, MethodInfo method, CustomAttributeContainer[] attributes, CustomParameterInfo[] parameters)
        {
            var index = new StackIndex();

            EmitAspectObjects(emitIl, attributes);
            EmitAspectMethodInterception(emitIl, index, method, new Type[] { method.ReturnType }, attributes, parameters);

            if (method.ReturnType != typeof(void))
            {
                EmitOnMethodReturning(AspectAction.OnMethodReturning, emitIl, index, method.ReturnType, attributes);
                EmitOnMethodReturned(emitIl, index, method.ReturnType, attributes);
                emitIl.Emit(OpCodes.Ldloc_S, index.Output);
            }

            emitIl.Emit(OpCodes.Ret);
        }

        // Creates call the the executed event
        private void EmitOnMethodExecuted(ILGenerator emitIl, StackIndex index, CustomAttributeContainer[] attributes, CustomParameterInfo[] parameters)
        {
            var len = attributes.Length;

            var paramCount = parameters.Length;

            for (var i = 0; i < len; i++)
            {
                emitIl.Emit(OpCodes.Ldloc, i);
                emitIl.Emit(OpCodes.Ldloc, index.Context);
                emitIl.Emit(OpCodes.Ldc_I4, paramCount);
                emitIl.Emit(OpCodes.Newarr, typeof(object));
                emitIl.Emit(OpCodes.Stloc, index.Store);

                for (var j = 0; j < paramCount; j++)
                {
                    emitIl.Emit(OpCodes.Ldloc, index.Store);
                    emitIl.Emit(OpCodes.Ldc_I4, j);
                    emitIl.Emit(OpCodes.Ldarg, j + 1);

                    var parameterType = parameters[j].ParameterType;
                    if (parameterType.GetTypeInfo().IsValueType) //not reference
                        emitIl.Emit(OpCodes.Box, parameterType);

                    emitIl.Emit(OpCodes.Stelem_Ref);
                }

                emitIl.Emit(OpCodes.Ldloc, index.Store);
                emitIl.EmitCall(OpCodes.Callvirt, typeof(BaseAspect).GetMethod("OnMethodExecuted"), new Type[] { typeof(object[]) });
            }
        }

        // Creates the "AND" condition test that all executing aspects must return true from for root method execution
        private void EmitAspectMethodInterception(ILGenerator emitIl, StackIndex index, MethodInfo method, Type[] types, CustomAttributeContainer[] attributes, CustomParameterInfo[] parameters)
        {
            var paramCount = parameters.Length;
            var len = attributes.Length;
            var isVoid = method.ReturnType == typeof(void);

            var trueLabel = emitIl.DefineLabel();
            var falseLabel = emitIl.DefineLabel();
            var endLabel = emitIl.DefineLabel();

            index.Context = emitIl.DeclareLocal(typeof(AspectEventContext));
            index.Store = emitIl.DeclareLocal(typeof(object[]));
            index.Bool = emitIl.DeclareLocal(typeof(bool));

            if (!isVoid)
            {
                index.Output = emitIl.DeclareLocal(method.ReturnType);
            }

            EmitEventParameters(emitIl, method.Name, index.Context);

            if (!isVoid)
            {
                if (method.ReturnType.GetTypeInfo().IsValueType)
                {
                    emitIl.LoadForValueType(0);
                }
                else
                {
                    emitIl.LoadForValueType(null);
                }
                emitIl.Emit(OpCodes.Stloc, index.Output);
            }

            for (var i = 0; i < len; i++)
            {
                emitIl.Emit(OpCodes.Ldloc, i);
                emitIl.Emit(OpCodes.Ldloc, index.Context);

                emitIl.Emit(OpCodes.Ldc_I4, paramCount);
                emitIl.Emit(OpCodes.Newarr, typeof(object));
                emitIl.Emit(OpCodes.Stloc, index.Store);

                for (var j = 0; j < paramCount; j++)
                {
                    emitIl.Emit(OpCodes.Ldloc, index.Store);
                    emitIl.Emit(OpCodes.Ldc_I4, j);
                    emitIl.Emit(OpCodes.Ldarg, j + 1);

                    var parameterType = parameters[j].ParameterType;
                    if (parameterType.GetTypeInfo().IsValueType) //not reference
                    {
                        emitIl.Emit(OpCodes.Box, parameterType);
                    }

                    emitIl.Emit(OpCodes.Stelem_Ref);
                }

                emitIl.Emit(OpCodes.Ldloc, index.Store);
                emitIl.EmitCall(OpCodes.Callvirt, typeof(BaseAspect).GetMethod("OnMethodExecuting"), new Type[] { typeof(AspectEventContext), typeof(object[]) });

                if (len > 1)
                {
                    if (i < len - 1)
                    {
                        emitIl.Emit(OpCodes.Brfalse, falseLabel);
                    }
                    else
                    {
                        emitIl.Emit(OpCodes.Ldc_I4_0);
                        emitIl.Emit(OpCodes.Ceq);
                        emitIl.Emit(OpCodes.Br, trueLabel);
                    }
                }
                else
                {
                    emitIl.Emit(OpCodes.Ldc_I4_0);
                    emitIl.Emit(OpCodes.Ceq);
                }
            }

            if (len > 1)
            {
                emitIl.MarkLabel(falseLabel);
                emitIl.Emit(OpCodes.Ldc_I4_1);
                emitIl.MarkLabel(trueLabel);
            }

            emitIl.Emit(OpCodes.Stloc, index.Bool);
            emitIl.Emit(OpCodes.Ldloc, index.Bool);
            emitIl.Emit(OpCodes.Brtrue, endLabel);
            emitIl.Emit(OpCodes.Nop);

            emitIl.Emit(OpCodes.Ldarg_0);

            for (var i = 1; i <= parameters.Length; i++)
            {
                emitIl.Emit(OpCodes.Ldarg_S, i);
            }

            emitIl.EmitCall(OpCodes.Call, method, types);

            if (!isVoid)
            {
                emitIl.Emit(OpCodes.Stloc, index.Output);
            }

            EmitOnMethodExecuted(emitIl, index, attributes, parameters);
            emitIl.Emit(OpCodes.Nop);

            emitIl.MarkLabel(endLabel);
        }

        // Creates call the the returning event
        private void EmitOnMethodReturning(AspectAction action, ILGenerator emitIl, StackIndex index, Type returnType, CustomAttributeContainer[] attributes)
        {
            var len = attributes.Length;
            var returnInfo = returnType.GetTypeInfo();

            for (var i = 0; i < len; i++)
            {
                emitIl.Emit(OpCodes.Ldloc_S, i);
                emitIl.Emit(OpCodes.Ldloc_S, index.Context);
                emitIl.Emit(OpCodes.Ldloc_S, index.Output);

                if (returnInfo.IsValueType)
                {
                    emitIl.Emit(OpCodes.Box, returnType);
                }

                emitIl.EmitCall(OpCodes.Callvirt, typeof(BaseAspect).GetMethod(action.ToString()), new Type[] { typeof(AspectEventContext), returnType });

                if (returnInfo.IsValueType)
                {
                    emitIl.Emit(OpCodes.Unbox_Any, returnType);
                }

                emitIl.Emit(OpCodes.Stloc_S, index.Output);
                emitIl.Emit(OpCodes.Nop);
            }
        }

        // Creates call the the returned event
        private void EmitOnMethodReturned(ILGenerator emitIl, StackIndex index, Type returnType, CustomAttributeContainer[] attributes)
        {
            var len = attributes.Length;
            var returnInfo = returnType.GetTypeInfo();

            for (var i = 0; i < len; i++)
            {
                emitIl.Emit(OpCodes.Ldloc_S, i);
                emitIl.Emit(OpCodes.Ldloc_S, index.Context);
                emitIl.Emit(OpCodes.Ldloc_S, index.Output);

                if (returnInfo.IsValueType) //not reference
                    emitIl.Emit(OpCodes.Box, returnType);

                emitIl.EmitCall(OpCodes.Callvirt, typeof(BaseAspect).GetMethod("OnMethodReturned"), new Type[] { returnType });
                emitIl.Emit(OpCodes.Nop);
            }
        }

        // Creates block wrapping the setter execution
        private void EmitOnPropertySetting(ILGenerator emitIl, Type baseType, Type propertyType, string propertyName, CustomAttributeContainer[] attributes, CustomParameterInfo[] parameters)
        {
            var index = new StackIndex();

            EmitAspectObjects(emitIl, attributes);
            EmitAspectSetterInterception(emitIl, index, baseType.GetMethod(propertyName), propertyName, new Type[] { propertyType }, attributes, parameters);

            emitIl.Emit(OpCodes.Ret);
        }

        // Creates the "AND" condition test that all executing aspects must return true from for root property setter execution
        private void EmitAspectSetterInterception(ILGenerator emitIl, StackIndex index, MethodInfo method, string methodName, Type[] types, CustomAttributeContainer[] attributes, CustomParameterInfo[] parameters)
        {
            var len = attributes.Length;
            var setterType = parameters.First().ParameterType;
            var setterInfo = setterType.GetTypeInfo();

            var trueLabel = emitIl.DefineLabel();
            var falseLabel = emitIl.DefineLabel();
            var endLabel = emitIl.DefineLabel();

            index.Context = emitIl.DeclareLocal(typeof(AspectEventContext));
            index.Store = emitIl.DeclareLocal(typeof(bool));

            EmitEventParameters(emitIl, methodName, index.Context);

            for (var i = 0; i < len; i++)
            {
                emitIl.Emit(OpCodes.Ldloc, i);
                emitIl.Emit(OpCodes.Ldloc, index.Context);
                emitIl.Emit(OpCodes.Ldarg, 1);

                if (setterInfo.IsValueType) //not reference
                {
                    emitIl.Emit(OpCodes.Box, setterType);
                }

                emitIl.EmitCall(OpCodes.Callvirt, typeof(BaseAspect).GetMethod("OnPropertySetting"), new Type[] { typeof(AspectEventContext), typeof(object) });

                if (len > 1)
                {
                    if (i < len - 1)
                    {
                        emitIl.Emit(OpCodes.Brfalse, falseLabel);
                    }
                    else
                    {
                        emitIl.Emit(OpCodes.Ldc_I4_0);
                        emitIl.Emit(OpCodes.Ceq);
                        emitIl.Emit(OpCodes.Br, trueLabel);
                    }
                }
                else
                {
                    emitIl.Emit(OpCodes.Ldc_I4_0);
                    emitIl.Emit(OpCodes.Ceq);
                }
            }

            if (len > 1)
            {
                emitIl.MarkLabel(falseLabel);
                emitIl.Emit(OpCodes.Ldc_I4_1);
                emitIl.MarkLabel(trueLabel);
            }

            emitIl.Emit(OpCodes.Stloc, index.Store);
            emitIl.Emit(OpCodes.Ldloc, index.Store);
            emitIl.Emit(OpCodes.Brtrue, endLabel);
            emitIl.Emit(OpCodes.Nop);

            emitIl.Emit(OpCodes.Ldarg_0);
            emitIl.Emit(OpCodes.Ldarg_1);
            emitIl.EmitCall(OpCodes.Call, method, types);

            emitIl.Emit(OpCodes.Nop);
            emitIl.MarkLabel(endLabel);
        }

        // Creates block wrapping the getter execution
        private void EmitOnPropertyGetting(ILGenerator emitIl, Type baseType, Type propertyType, string propertyName, CustomAttributeContainer[] attributes)
        {
            var index = new StackIndex();

            EmitAspectObjects(emitIl, attributes);

            index.Store = index.Output = emitIl.DeclareLocal(propertyType);
            index.Context = emitIl.DeclareLocal(typeof(AspectEventContext));

            EmitEventParameters(emitIl, propertyName, index.Context);

            emitIl.Emit(OpCodes.Ldarg_0);
            emitIl.EmitCall(OpCodes.Call, baseType.GetMethod(propertyName), null);
            emitIl.Emit(OpCodes.Stloc_S, index.Store);

            EmitOnMethodReturning(AspectAction.OnPropertyGetting, emitIl, index, propertyType, attributes);

            emitIl.Emit(OpCodes.Ldloc_S, index.Output);
            emitIl.Emit(OpCodes.Ret);
        }
    }
}
