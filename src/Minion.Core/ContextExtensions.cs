using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Minion
{
	public static class ContextExtensions
	{
		public static Assembly GetAssembly(this ActionExecutingContext context)
		{
			var controller = (Controller)context.Controller;
			return controller.GetAssembly();
		}

		public static Assembly GetAssembly(this ResultExecutedContext context)
		{
			var controller = (Controller)context.Controller;
			return controller.GetAssembly();
		}

		public static Assembly GetAssembly(this Controller controller)
		{
			var action = controller
				.ControllerContext
				.ActionDescriptor;

			var assembly = action
				.ControllerTypeInfo
				.Assembly;

			return assembly;
		}

		public static TService GetService<TService>(this FilterContext context)
		{
			return context.HttpContext.GetService<TService>();
		}

		public static TService GetService<TService>(this HttpContext context)
		{
			return context.RequestServices.GetService<TService>();
		}

		public static TService GetService<TService>(this IServiceProvider provider)
		{
			var output = (TService)provider.GetService(typeof(TService));
			return output;
		}

		public static IDictionary<object, object> Items(this FilterContext context)
		{
			return context.HttpContext.Items;
		}

		public static object Retrieve(this FilterContext context, object key)
		{
			object output;
			context.HttpContext.Items.TryGetValue(key, out output);

			return output;
		}

		public static void Store(this FilterContext context, object key, object value)
		{
			context.HttpContext.Items.Add(key, value);
		}

	}
}
