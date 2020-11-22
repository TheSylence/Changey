using Changey.Models;
using CommandLine;

namespace Changey.Options
{
	[Verb("security", HelpText = "Adds a new change to the 'Security' section of the current unreleased version")]
	internal class SecurityOption : SectionOption
	{
		public SecurityOption(string message, bool verbose, bool silent, string path)
			: base(message, verbose, silent, path)
		{
		}

		public override Section Section => Section.Security;
	}
}