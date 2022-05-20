using CommandLine;

namespace Changey.Options;

[Verb("yank", HelpText = "Yanks the latest released version in the changelog")]
internal class YankOption : BaseOption
{
	public YankOption(string path, bool silent, bool verbose)
		: base(path, silent, verbose)
	{
	}
}