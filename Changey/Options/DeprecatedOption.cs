using Changey.Models;
using CommandLine;

namespace Changey.Options
{
	[Verb("deprecate", HelpText = "Adds a new change to the 'Deprecated' section of the current unreleased version")]
	internal class DeprecatedOption : SectionOption
	{
		public DeprecatedOption(string message, bool verbose, bool silent, string path)
			: base(message, verbose, silent, path)
		{
		}

		public override Section Section => Section.Deprecated;
	}
}