using System;

namespace Minion.DataAccess
{
	[Flags]
	public enum FormCommand
	{
		Save = 1,
		Submit = 1<<1,
		SaveCopy = 1<<2,
		New = 1<<3,
		Reset = 1<<4,
		Delete = 1<<5,
		Search = 1<<6
	}
}