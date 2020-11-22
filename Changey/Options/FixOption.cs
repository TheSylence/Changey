using Changey.Models;
using CommandLine;

namespace Changey.Options
{
	[Verb("fix", HelpText = "Adds a new change to the 'Fixed' section of the current unreleased version")]
	internal class FixOption : SectionOption
	{
		public FixOption(string message, bool verbose, bool silent, string path)
			: base(message, verbose, silent, path)
		{
		}

		public override Section Section => Section.Fixed;
	}
}