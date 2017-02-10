using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Minion.Emit.Aspects;

namespace Minion.Emit
{
	public class AspectEmitter : IEmitter
	{
		private const string MODULE_NAME = "Minion.Aspects.Types.dll";
		private readonly ITypeCache _cache;

		public AspectEmitter(ITypeCache cache)
		{
			_cache = cache;
		}

		public TObject Materialize<TObject>() where TObject : class, new()
		{
			return Materialize<TObject>((object[])null);
		}

		public TObject Materialize<TObject>(params object[] parameters)
			where TObject : class
		{
			var targetType = _cache.GetType<TObject>(x => x.Aspect, this);
			var output = Activator.CreateInstance(targetType, parameters);

			return (TObject)output;
		}

		public TObject Materialize<TObject>(Expression<Func<TObject>> initiator)
			where TObject : class
		{
			var body = (NewExpression)initiator.Body;
			var args = body.Arguments.Select(x => x.ExtractValue()).ToArray();
			var targetType = _cache.GetType<TObject>(x => x.Aspect, this);
			var proxy = Activator.CreateInstance(targetType, args);

			return (TObject)proxy;
		}

		public Type GenerateType<T>()
			where T : class
		{
			return GenerateType(typeof(T));
		}

		public Type GenerateType(Type baseType)
		{
			if (baseType.IsInterface)
			{
				throw new Exception("For aspect interception the type must be an implementation");
			}

			var naming = new ScopeNamespace(baseType);
			var assemblyName = new AssemblyName() { Name = naming.Key };
			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
			var modBuilder = assemblyBuilder.DefineDynamicModule(MODULE_NAME);
			var typeAttr = TypeAttributes.Class | TypeAttributes.Public;

			var typeBuilder = modBuilder.DefineType(naming.Key + "Proxy", typeAttr, baseType);
			typeBuilder.AddInterfaceImplementation(typeof(IAspect));

			GenConstuctors(baseType, typeBuilder);
			GenProperties(baseType, typeBuilder);
			GenMethods(baseType, typeBuilder);

			var targetType = typeBuilder.CreateType();

			//assemblyBuilder.Save(MODULE_NAME);

			return targetType;
		}

		private void GenConstuctors(Type baseType, TypeBuilder builder)
		{
			foreach (var constructor in baseType.GetConstructors(BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
			{
				var parameters = constructor.GetParameters();
				var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();

				var ctorBuilder = builder.DefineConstructor(constructor.Attributes, constructor.CallingConvention, parameterTypes);
				var ctorIl = ctorBuilder.GetILGenerator();
				var paramCount = parameters.Length;

				ctorIl.Emit(OpCodes.Ldarg_0);

				for (var i = 1; i <= paramCount; ++i)
				{
					ctorIl.Emit(OpCodes.Ldarg, i);
				}

				ctorIl.Emit(OpCodes.Call, constructor);
				ctorIl.Emit(OpCodes.Ret);
			}
		}

		private void GenProperties(Type baseType, TypeBuilder typeBuilder, IEnumerable<PropertyInfo> properties = null)
		{
			var getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final;

			if (properties == null)
				properties = baseType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

			foreach (var prop in properties)
			{
				var attributes = EmitHelpers.GetAttributeContainers(prop.GetCustomAttributes(true), prop.GetCustomAttributesData()).ToArray();

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

		private void GenMethods(Type baseType, TypeBuilder builder)
		{
			var methods = baseType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

			foreach (var method in methods)
			{
				var attributes = EmitHelpers.GetAttributeContainers(method.GetCustomAttributes(true), method.GetCustomAttributesData()).ToArray();

				if (attributes.Length == 0 || method.Name.Contains("get_") || method.Name.Contains("set_") || method.Module.Name == "mscorlib.dll")
					continue;

				var methodAttributes = method.Attributes ^ MethodAttributes.VtableLayoutMask;
				var cpi = method.GetParameters().ToCustomParameters();
				var parameterTypes = cpi.Select(x => x.ParameterType).ToArray();

				var methodBuilder = builder.DefineMethod(method.Name, methodAttributes, CallingConventions.HasThis, method.ReturnType, parameterTypes);
				var methodIl = methodBuilder.GetILGenerator();

				EmitOnMethodExecution(methodIl, baseType, method, attributes, cpi);
			}
		}

		private void EmitAspectObjects(ILGenerator emitIl, CustomAttributeContainer[] attributes)
		{
			var len = attributes.Count();

			if (len == 0)
				return;

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

		private void EmitAspectSetterInterception(ILGenerator emitIl, StackIndex index, MethodInfo method, string methodName, Type[] types, CustomAttributeContainer[] attributes, CustomParameterInfo[] parameters)
		{
			var len = attributes.Length;

			var setterType = parameters.First().ParameterType;

			var trueLabel = emitIl.DefineLabel();
			var falseLabel = emitIl.DefineLabel();
			var endLabel = emitIl.DefineLabel();

			index.Context = emitIl.DeclareLocal(typeof(AspectEventContext));
			index.Store = emitIl.DeclareLocal(typeof(bool));

			////////////////////////////////////
			emitIl.Emit(OpCodes.Ldarg_0);
			emitIl.Emit(OpCodes.Ldstr, methodName);
			emitIl.EmitCall(OpCodes.Call, typeof(EmitHelpers).GetMethod("CreateAspectContext", BindingFlags.Public | BindingFlags.Static), new Type[] { typeof(object), typeof(string) });
			emitIl.Emit(OpCodes.Stloc, index.Context);
			///////////////////////////////////

			for (var i = 0; i < len; i++)
			{
				emitIl.Emit(OpCodes.Ldloc, i);
				emitIl.Emit(OpCodes.Ldloc, index.Context);
				emitIl.Emit(OpCodes.Ldarg, 1);

				if (setterType.IsValueType) //not reference
					emitIl.Emit(OpCodes.Box, setterType);

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
				index.Output = emitIl.DeclareLocal(method.ReturnType);

			////////////////////////////////////
			emitIl.Emit(OpCodes.Ldarg_0);
			emitIl.Emit(OpCodes.Ldstr, method.Name);
			emitIl.EmitCall(OpCodes.Call, typeof(EmitHelpers).GetMethod("CreateAspectContext", BindingFlags.Public | BindingFlags.Static), new Type[] { typeof(object), typeof(string) });
			emitIl.Emit(OpCodes.Stloc, index.Context);
			///////////////////////////////////

			if (!isVoid)
			{
				if (method.ReturnType.IsValueType)
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
					if (parameterType.IsValueType) //not reference
						emitIl.Emit(OpCodes.Box, parameterType);

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
				emitIl.Emit(OpCodes.Stloc, index.Output);

			EmitOnMethodExecuted(emitIl, index, attributes, parameters);
			emitIl.Emit(OpCodes.Nop);

			emitIl.MarkLabel(endLabel);

		}

		private void EmitOnMethodReturned(ILGenerator emitIl, StackIndex index, Type returnType, CustomAttributeContainer[] attributes)
		{
			var len = attributes.Length;

			for (var i = 0; i < len; i++)
			{
				emitIl.Emit(OpCodes.Ldloc_S, i);
				emitIl.Emit(OpCodes.Ldloc_S, index.Context);
				emitIl.Emit(OpCodes.Ldloc_S, index.Output);

				if (returnType.IsValueType) //not reference
					emitIl.Emit(OpCodes.Box, returnType);

				emitIl.EmitCall(OpCodes.Callvirt, typeof(BaseAspect).GetMethod("OnMethodReturned"), new Type[] { returnType });
				emitIl.Emit(OpCodes.Nop);
			}
		}

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
					if (parameterType.IsValueType) //not reference
						emitIl.Emit(OpCodes.Box, parameterType);

					emitIl.Emit(OpCodes.Stelem_Ref);
				}

				emitIl.Emit(OpCodes.Ldloc, index.Store);
				emitIl.EmitCall(OpCodes.Callvirt, typeof(BaseAspect).GetMethod("OnMethodExecuted"), new Type[] { typeof(object[]) });
			}
		}

		private void EmitOnMethodReturning(AspectAction action, ILGenerator emitIl, StackIndex index, Type returnType, CustomAttributeContainer[] attributes)
		{
			var len = attributes.Length;

			for (var i = 0; i < len; i++)
			{
				emitIl.Emit(OpCodes.Ldloc_S, i);
				emitIl.Emit(OpCodes.Ldloc_S, index.Context);
				emitIl.Emit(OpCodes.Ldloc_S, index.Output);

				if (returnType.IsValueType)
				{
					//not reference
					emitIl.Emit(OpCodes.Box, returnType);
				}

				emitIl.EmitCall(OpCodes.Callvirt, typeof(BaseAspect).GetMethod(action.ToString()), new Type[] { typeof(AspectEventContext), returnType });

				if (returnType.IsValueType)
				{
					//not reference
					emitIl.Emit(OpCodes.Unbox_Any, returnType);
				}

				emitIl.Emit(OpCodes.Stloc_S, index.Output);
				emitIl.Emit(OpCodes.Nop);
			}
		}

		private void EmitOnPropertySetting(ILGenerator emitIl, Type baseType, Type propertyType, string propertyName, CustomAttributeContainer[] attributes, CustomParameterInfo[] parameters)
		{
			var index = new StackIndex();

			EmitAspectObjects(emitIl, attributes);
			EmitAspectSetterInterception(emitIl, index, baseType.GetMethod(propertyName), propertyName, new Type[] { propertyType }, attributes, parameters);

			emitIl.Emit(OpCodes.Ret);
		}

		private void EmitOnPropertyGetting(ILGenerator emitIl, Type baseType, Type propertyType, string propertyName, CustomAttributeContainer[] attributes)
		{
			var index = new StackIndex();

			EmitAspectObjects(emitIl, attributes);

			index.Store = index.Output = emitIl.DeclareLocal(propertyType);
			index.Context = emitIl.DeclareLocal(typeof(AspectEventContext));

			////////////////////////////////////
			emitIl.Emit(OpCodes.Ldarg_0);
			emitIl.Emit(OpCodes.Ldstr, propertyName);
			emitIl.EmitCall(OpCodes.Call, typeof(EmitHelpers).GetMethod("CreateAspectContext", BindingFlags.Public | BindingFlags.Static), new Type[] { typeof(object), typeof(string) });
			emitIl.Emit(OpCodes.Stloc, index.Context);
			///////////////////////////////////

			emitIl.Emit(OpCodes.Ldarg_0);
			emitIl.EmitCall(OpCodes.Call, baseType.GetMethod(propertyName), null);
			emitIl.Emit(OpCodes.Stloc_S, index.Store);

			EmitOnMethodReturning(AspectAction.OnPropertyGetting, emitIl, index, propertyType, attributes);

			emitIl.Emit(OpCodes.Ldloc_S, index.Output);
			emitIl.Emit(OpCodes.Ret);
		}

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
	}

	internal class CustomParameterInfo
	{
		public ParameterAttributes Attributes { get; set; }
		public MemberInfo Member { get; set; }
		public Type ParameterType { get; set; }
	}

	internal class StackIndex
	{
		public LocalBuilder Output { get; set; }
		public LocalBuilder Store { get; set; }
		public LocalBuilder Bool { get; set; }
		public LocalBuilder Context { get; set; }
	}

	public enum AspectAction
	{
		OnClassInitialized,
		OnMethodExecuting,
		OnMethodExecuted,
		OnMethodReturning,
		OnMethodReturned,
		OnPropertySetting,
		OnPropertyGetting
	}
}
