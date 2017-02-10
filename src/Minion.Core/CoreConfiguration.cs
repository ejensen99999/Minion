using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minion.Configuration;

namespace Minion
{
	public class CoreConfiguration : ICoreConfiguration
	{
		private readonly ILogger<CoreConfiguration> _coreLog;

		public Guid CoreId { get; }
		public Guid MenuCoreId { get; }
		public int Parallelism { get; private set; }

		public ActiveDirectorySettings ActiveDirectory { get; }
		public BusSettings Bus { get; }
		public CacheSettings Cache { get; }
		public ConnectionSettings Connection { get; }
		public CronSettings Cron { get; }
		public EmailSettings Email { get; }
		public FaxSettings Fax { get; }
		public FileSettings Files { get; }
		public FTPSettings FTP { get; }
		public PrintSettings Print { get; }

        //public CoreConfiguration(
        //	ILogger<CoreConfiguration> log,
        //	IOptions<ActiveDirectorySettings> activeDirectory,
        //	IOptions<BusSettings> bus,
        //	IOptions<CacheSettings> cache,
        //	IOptions<ConnectionSettings> connection,
        //	IOptions<CronSettings> cron,
        //	IOptions<EmailSettings> email,
        //	IOptions<FaxSettings> fax,
        //	IOptions<FileSettings> files,
        //	IOptions<FTPSettings> ftp,
        //	IOptions<PrintSettings> print
        //	)
        //{
        //	_coreLog = log;

        //	CoreId = Guid.Parse("BC0C4AF8-5EB2-4C3A-B640-5357428C0557");
        //	MenuCoreId = Guid.Parse("039BB7F3-AAB2-45C4-B4A4-25E20A70E0AE");
        //	ActiveDirectory = activeDirectory.Value;
        //	Bus = bus.Value;
        //	Cache = cache.Value;
        //	Connection = connection.Value;
        //	Cron = cron.Value;
        //	Email = email.Value;
        //	Fax = fax.Value;
        //	Files = files.Value;
        //	FTP = ftp.Value;
        //	Print = print.Value;
        //}

        public CoreConfiguration(
            ILoggerFactory log,
            ActiveDirectorySettings activeDirectory,
            BusSettings bus,
            CacheSettings cache,
            ConnectionSettings connection,
            CronSettings cron,
            EmailSettings email,
            FaxSettings fax,
            FileSettings files,
            FTPSettings ftp,
            PrintSettings print
            )
        {
            _coreLog = log.CreateLogger<CoreConfiguration>();

            CoreId = Guid.Parse("BC0C4AF8-5EB2-4C3A-B640-5357428C0557");
            MenuCoreId = Guid.Parse("039BB7F3-AAB2-45C4-B4A4-25E20A70E0AE");
            ActiveDirectory = activeDirectory;
            Bus = bus;
            Cache = cache;
            Connection = connection;
            Cron = cron;
            Email = email;
            Fax = fax;
            Files = files;
            FTP = ftp;
            Print = print;
        }
    }
}