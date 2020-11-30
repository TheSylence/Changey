using Changey.Models;
using CommandLine;

namespace Changey.Options
{
	[Verb("add", HelpText = "Adds a new change to the 'Added' section of the current unreleased version")]
	internal class AddOption : SectionOption
	{
		public AddOption(string message, string path, bool silent, bool verbose)
			: base(message, path, silent, verbose)
		{
		}

		internal override Section Section => Section.Added;
	}
}