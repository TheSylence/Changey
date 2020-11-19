using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Changey.Services;
using CommandLine;

namespace Changey.Commands
{
	[Verb("init", HelpText = "Create a new changelog")]
	internal class InitCommand : BaseCommand
	{
		[ExcludeFromCodeCoverage]
		public InitCommand(bool semVer, bool verbose, bool silent, string path)
			: this(semVer, verbose, silent, path, null)
		{
		}

		public InitCommand(bool semVer, bool verbose, bool silent, string path, IChangeLogCreator? changeLogCreator)
			: base(verbose, silent, path)
		{
			SemVer = semVer;

			_changeLogCreator =
				changeLogCreator ?? new ChangeLogCreator(Logger, new ChangeLogSerializer(new FileAccess()));
		}

		[Option(HelpText = "Indicates the project uses semver", Default = true)]
		public bool SemVer { get; }

		public override async Task Execute()
		{
			try
			{
				Logger.Verbose($"Creating changelog at {Path}");
				await _changeLogCreator.CreateChangelog(Path, SemVer);
				Logger.Verbose($"Created changelog at {Path}");
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to create changelog", ex);
			}
		}

		private readonly IChangeLogCreator _changeLogCreator;
	}
}