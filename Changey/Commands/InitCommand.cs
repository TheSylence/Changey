using System.Threading.Tasks;
using Changey.Models;
using CommandLine;

namespace Changey.Commands
{
	[Verb("init", HelpText = "Create a new changelog")]
	internal class InitCommand : BaseCommand
	{
		public InitCommand(bool semVer, bool verbose, bool silent, string path)
			: this(semVer, verbose, silent, path, null)
		{
		}

		public InitCommand(bool semVer, bool verbose, bool silent, string path, IChangeLogCreator? changeLogCreator)
			: base(verbose, silent, path)
		{
			SemVer = semVer;

			_changeLogCreator = changeLogCreator ?? new ChangeLogCreator(Logger, new ChangeLogSerializer());
		}

		[Option(HelpText = "Indicates the project uses semver", Default = true)]
		public bool SemVer { get; }

		public override async Task Execute()
		{
			await _changeLogCreator.CreateChangelog(Path, SemVer);
		}

		private readonly IChangeLogCreator _changeLogCreator;
	}
}