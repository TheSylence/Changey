using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Changey.Services;
using CommandLine;

namespace Changey.Commands
{
	[Verb("release", HelpText = "Marks the latest unreleased version as released")]
	internal class ReleaseCommand : BaseCommand
	{
		[ExcludeFromCodeCoverage]
		public ReleaseCommand(string? date, string name, string path, bool silent, bool verbose)
			: this(date, name, path, silent, verbose, null)
		{
		}

		public ReleaseCommand(string? date, string name, string path,
			bool silent, bool verbose,
			IChangeLogReleaser? changeLogReleaser)
			: base(verbose, silent, path)
		{
			Name = name;
			Date = date;

			_changeLogReleaser =
				changeLogReleaser ?? new ChangeLogReleaser(Logger, new ChangeLogSerializer(new FileAccess()));
		}

		[Option('d', HelpText = "The date to use for this release. If omitted the current date will be used",
			Default = null)]
		public string? Date { get; }

		[Option('n', HelpText = "Name of the version to release.", Required = true)]
		public string Name { get; }

		public override async Task Execute()
		{
			try
			{
				if (!await _changeLogReleaser.Release(Path, Date, Name))
					Logger.Warning("Could not release version");
				else
					Logger.Verbose("Released version");
			}
			catch (Exception exception)
			{
				Logger.Error("Failed to release version", exception);
			}
		}

		private readonly IChangeLogReleaser _changeLogReleaser;
	}
}