using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Minion.Ioc.Interfaces;
using Minion.Ioc.Models;
using System.Linq;

namespace Minion.Ioc.Profiler
{
    public class ConstructorEmitter
	{
		private const string ModuleName = "Minion.Ioc.Constructors.dll";

		public static IConstructor Emit(Type concrete, IParameterProfile profile)
		{
			var assemblyType = "IocConstructor";
			var parameterTypes = profile.Parameters;

			var assemblyName = new AssemblyName{ Name = $"{concrete.FullName}.{assemblyType}" };
			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
			var modBuilder = assemblyBuilder.DefineDynamicModule(ModuleName);
		
			var typeAttr = TypeAttributes.Class
			               | TypeAttributes.Public
			               | TypeAttributes.BeforeFieldInit
			               | TypeAttributes.AutoClass;

			var typeBuilder = modBuilder.DefineType(assemblyType, typeAttr);
			typeBuilder.AddInterfaceImplementation(typeof(IConstructor));

			GenerateConstructor(typeBuilder);
			GenerateConstruct(profile.Ctor, typeBuilder, parameterTypes);

			var targetType = typeBuilder.CreateTypeInfo();
			var target = (IConstructor) Activator.CreateInstance(targetType.AsType());
			return target;
		}

		private static void GenerateConstructor(TypeBuilder builder)
		{
			var methodAtts = MethodAttributes.Public
				| MethodAttributes.HideBySig
				| MethodAttributes.SpecialName
				| MethodAttributes.RTSpecialName;

			builder.DefineDefaultConstructor(methodAtts);
		}

		private static void GenerateConstruct(ConstructorInfo ctor, TypeBuilder builder, List<ParameterDefinition> parameterTypes)
		{
			var methodAtts = MethodAttributes.Public 
				| MethodAttributes.Virtual 
				| MethodAttributes.HideBySig 
				| MethodAttributes.NewSlot;

			var method = typeof(List<object>).GetMethod("get_Item", new[] { typeof(int) });

			var methodBuilder = builder.DefineMethod("Construct",
				methodAtts, 
				CallingConventions.HasThis,
				typeof(object), 
				new [] { typeof(List<object>) });

			var methodIl = methodBuilder.GetILGenerator();
			var output = methodIl.DeclareLocal(typeof(object));
			var stuff = methodIl.DefineLabel();

			methodIl.Emit(OpCodes.Nop);

			for (var i = 0; i < parameterTypes.Count; i++)
			{
				methodIl.Emit(OpCodes.Ldarg_1);
				methodIl.Emit(OpCodes.Ldc_I4, i);
				methodIl.EmitCall(OpCodes.Callvirt, method, null);
				methodIl.Emit(OpCodes.Unbox_Any, parameterTypes[i].ContractType);
			}

			methodIl.Emit(OpCodes.Newobj, ctor);

			methodIl.Emit(OpCodes.Stloc_S, output);
			methodIl.Emit(OpCodes.Br_S, stuff);
			methodIl.MarkLabel(stuff);
			methodIl.Emit(OpCodes.Ldloc_S, output);
			methodIl.Emit(OpCodes.Ret);
		}
	}
}
