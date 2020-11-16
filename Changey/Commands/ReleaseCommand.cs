using System.IO;
using System.Threading.Tasks;
using Changey.Models;
using CommandLine;

namespace Changey.Commands
{
	[Verb("release", HelpText = "Marks the latest unreleased version as released")]
	internal class ReleaseCommand : BaseCommand
	{
		public ReleaseCommand(string version, string date, bool verbose, bool silent, string path)
			: this(version, date, verbose, silent, path, null)
		{
		}

		public ReleaseCommand(string version, string date, bool verbose, bool silent, string path,
			IChangeLogReleaser? changeLogReleaser)
			: base(verbose, silent, path)
		{
			Version = version;
			Date = date;


			_changeLogReleaser = changeLogReleaser ?? new ChangeLogReleaser(Logger, new ChangeLogSerializer());
		}

		[Option('d', HelpText = "The date to use for this release. If omitted the current date will be used",
			Default = null)]
		public string Date { get; }

		[Option('v', HelpText = "Name of the version to release.")]
		public string Version { get; }

		public override async Task Execute()
		{
			try
			{
				if (!await _changeLogReleaser.Release(Path, Date, Version))
					Logger.Verbose("Could not release version");
				else
					Logger.Verbose("Released version");
			}
			catch (IOException exception)
			{
				Logger.Error("Failed to release version", exception);
			}
		}

		private readonly IChangeLogReleaser _changeLogReleaser;
	}
}