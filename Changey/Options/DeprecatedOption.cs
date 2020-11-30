using Changey.Models;
using CommandLine;

namespace Changey.Options
{
	[Verb("deprecate", HelpText = "Adds a new change to the 'Deprecated' section of the current unreleased version")]
	internal class DeprecatedOption : SectionOption
	{
		public DeprecatedOption(string message, string path, bool silent, bool verbose)
			: base(message, path, silent, verbose)
		{
		}

		internal override Section Section => Section.Deprecated;
	}
}