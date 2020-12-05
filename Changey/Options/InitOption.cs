using CommandLine;

namespace Changey.Options
{
	[Verb("init", HelpText = "Create a new changelog")]
	internal class InitOption : BaseOption
	{
		public InitOption(bool overwrite, bool semVer, string path, bool silent, bool verbose)
			: base(path, silent, verbose)
		{
			SemVer = semVer;
			Overwrite = overwrite;
		}

		[Option('o', HelpText = "Indicates whether an existing changelog should be overwritten", Default = false)]
		public bool Overwrite { get; }

		[Option(HelpText = "Indicates the project uses semver", Default = true)]
		public bool SemVer { get; }
	}
}