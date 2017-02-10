using System;
using System.Runtime.CompilerServices;

namespace Minion.Security
{
	public class Asset
	{
		public int AssetId
		{
			get;
			set;
		}

		public DateTimeOffset DateCreated
		{
			get;
			set;
		}

		public DateTimeOffset DateVerified
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public Guid ActionId
		{
			get;
			set;
		}

		public string ActionName
		{
			get;
			set;
		}

		public Guid ControllerId
		{
			get;
			set;
		}

		public string ControllerName
		{
			get;
			set;
		}

		public Guid? SuperiorId
		{
			get;
			set;
		}

		public bool Tombstone
		{
			get;
			set;
		}

		public SecurityAssetType Type
		{
			get;
			set;
		}

		public Guid UniqueId
		{
			get;
			set;
		}

		public string AssemblyName
		{
			get;
			set;
		}
	}
}