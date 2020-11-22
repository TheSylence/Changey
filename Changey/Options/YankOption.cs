using CommandLine;

namespace Changey.Options
{
	[Verb("yank", HelpText = "Yanks the latest released version in the changelog")]
	internal class YankOption : BaseOption
	{
		public YankOption(bool verbose, bool silent, string path)
			: base(path, silent, verbose)
		{
		}
	}
}