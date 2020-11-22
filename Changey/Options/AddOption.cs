using Changey.Models;
using CommandLine;

namespace Changey.Options
{
	[Verb("add", HelpText = "Adds a new change to the 'Added' section of the current unreleased version")]
	internal class AddOption : SectionOption
	{
		public AddOption(string message, bool verbose, bool silent, string path)
			: base(message, verbose, silent, path)
		{
		}

		public override Section Section => Section.Added;
	}
}