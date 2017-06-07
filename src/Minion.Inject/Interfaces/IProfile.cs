using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Minion.Inject.Interfaces
{
	public interface IProfile
	{
		Type Concrete { get; }
		Type Contract { get; }
		ServiceLifetime Lifecycle { get; }
		List<Type> Parameters { get; }
		IConstructor Constructor { get; }
		dynamic Instance { get; set; }
		object Initiate(Container container, List<object> parameters);
	}
}