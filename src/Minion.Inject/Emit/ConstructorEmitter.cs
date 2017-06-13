using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Minion.Inject.Aspects;
using Minion.Inject.Interfaces;

namespace Minion.Inject.Emit
{
    public class ConstructorEmitter
	{
		private const string ModuleName = "Minion.Inject.Constructors.dll";
	    private static readonly AspectEmitter _aspectEmitter;

	    static ConstructorEmitter()
	    {
	        _aspectEmitter = new AspectEmitter();
	    }

	    public static IConstructor Emit(Type concrete, ConstructorInfo ctor, IEnumerable<Type> parameters)
		{
		    if (concrete == null)
		    {
                return null;
		    }

		    if (concrete.InheritsFrom(typeof(IAspect)))
		    {
		        concrete = _aspectEmitter.CreateAspectProxyType(concrete, ctor);
		        ctor = concrete
                    .GetConstructors()
		            .First();
		    }

		    var assemblyType = "IocConstructor";

			var assemblyName = new AssemblyName{ Name = $"{concrete.FullName}.{assemblyType}" };
			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
			var modBuilder = assemblyBuilder.DefineDynamicModule(ModuleName);
		
			var typeAttr = TypeAttributes.Class
			               | TypeAttributes.Public
			               | TypeAttributes.BeforeFieldInit
			               | TypeAttributes.AutoClass;

			var typeBuilder = modBuilder.DefineType(assemblyType, typeAttr);
			typeBuilder.AddInterfaceImplementation(typeof(IConstructor));

			GenerateConstructor(ctor, typeBuilder, parameters.ToList());

			var targetType = typeBuilder.CreateTypeInfo();
			var target = (IConstructor) Activator.CreateInstance(targetType.AsType());

			return target;
		}

		private static void GenerateConstructor(ConstructorInfo ctor, TypeBuilder builder, List<Type> parameterTypes)
		{
			var ctorAtts = MethodAttributes.Public
				| MethodAttributes.HideBySig
				| MethodAttributes.SpecialName
				| MethodAttributes.RTSpecialName;

			builder.DefineDefaultConstructor(ctorAtts);

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

			for (var i = 0; i < parameterTypes.Count(); i++)
			{
				methodIl.Emit(OpCodes.Ldarg_1);
				methodIl.Emit(OpCodes.Ldc_I4, i);
				methodIl.EmitCall(OpCodes.Callvirt, method, null);
				methodIl.Emit(OpCodes.Unbox_Any, parameterTypes[i]);
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
