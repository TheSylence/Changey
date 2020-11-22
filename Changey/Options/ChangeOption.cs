using Changey.Models;
using CommandLine;

namespace Changey.Options
{
	[Verb("change", HelpText = "Adds a new change to the 'Changed' section of the current unreleased version")]
	internal class ChangeOption : SectionOption
	{
		public ChangeOption(string message, bool verbose, bool silent, string path)
			: base(message, verbose, silent, path)
		{
		}

		public override Section Section => Section.Changed;
	}
}