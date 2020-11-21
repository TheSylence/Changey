using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Changey.Services;
using CommandLine;

namespace Changey.Commands
{
	[Verb("yank", HelpText = "Yanks the latest released version in the changelog")]
	internal class YankCommand : BaseCommand
	{
		[ExcludeFromCodeCoverage]
		public YankCommand(bool verbose, bool silent, string path)
			: this(verbose, silent, path, null)
		{
		}

		public YankCommand(bool verbose, bool silent, string path, IVersionYanker? versionYanker)
			: base(verbose, silent, path)
		{
			_versionYanker = versionYanker ?? new VersionYanker(Logger, new ChangeLogSerializer(new FileAccess()));
		}

		public override async Task Execute()
		{
			await _versionYanker.Yank(Path);
		}

		private readonly IVersionYanker _versionYanker;
	}
}