using System;
using Minion.Configuration;

namespace Minion
{
	public interface ICoreConfiguration
	{
		Guid CoreId
		{
			get;
		}

		Guid MenuCoreId
		{
			get;
		}

		int Parallelism
		{
			get;
		}

		ActiveDirectorySettings ActiveDirectory
		{
			get;
		}

		BusSettings Bus
		{
			get;
		}

		CacheSettings Cache
		{
			get;
		}

		ConnectionSettings Connection
		{
			get;
		}

		CronSettings Cron
		{
			get;
		}

		EmailSettings Email
		{
			get;
		}

		FaxSettings Fax
		{
			get;
		}

		FileSettings Files
		{
			get;
		}

		FTPSettings FTP
		{
			get;
		}

		PrintSettings Print
		{
			get;
		}

	}
}