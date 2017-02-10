using System;

namespace Minion.DataControl.Interfaces
{
	public interface IMeasure
	{
		dynamic GetValue();
		//Type GetBaseType();
		Type MeasureType { get; }
	}
}
