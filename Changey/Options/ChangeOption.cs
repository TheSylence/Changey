using Changey.Models;
using CommandLine;

namespace Changey.Options
{
	[Verb("change", HelpText = "Adds a new change to the 'Changed' section of the current unreleased version")]
	internal class ChangeOption : SectionOption
	{
		public ChangeOption(string message, string path, bool silent, bool verbose)
			: base(message, path, silent, verbose)
		{
		}

		internal override Section Section => Section.Changed;
	}
}