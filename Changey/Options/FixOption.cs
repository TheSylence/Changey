using Changey.Models;
using CommandLine;

namespace Changey.Options
{
	[Verb("fix", HelpText = "Adds a new change to the 'Fixed' section of the current unreleased version")]
	internal class FixOption : SectionOption
	{
		public FixOption(string message, string path, bool verbose, bool silent)
			: base(message, path, silent, verbose)
		{
		}

		internal override Section Section => Section.Fixed;
	}
}