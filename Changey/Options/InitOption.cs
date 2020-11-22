using CommandLine;

namespace Changey.Options
{
	[Verb("init", HelpText = "Create a new changelog")]
	internal class InitOption : BaseOption
	{
		public InitOption(bool semVer, string path, bool silent, bool verbose)
			: base(path, silent, verbose)
		{
			SemVer = semVer;
		}

		[Option(HelpText = "Indicates the project uses semver", Default = true)]
		public bool SemVer { get; }
	}
}