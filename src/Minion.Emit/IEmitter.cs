using System;
using System.Linq.Expressions;

namespace Minion.Emit
{
	public interface IEmitter
	{
		TObject Materialize<TObject>(Expression<Func<TObject>> initiator)
			where TObject : class;

		TObject Materialize<TObject>() 
			where TObject : class, new();

		Type GenerateType<T>()
			where T : class;

		Type GenerateType(Type baseType);
	}
}
