using Changey.Models;
using CommandLine;

namespace Changey.Options
{
	[Verb("remove", HelpText = "Adds a new change to the 'Removed' section of the current unreleased version")]
	internal class RemoveOption : SectionOption
	{
		public RemoveOption(string message, string path, bool silent, bool verbose)
			: base(message, path, silent, verbose)
		{
		}

		internal override Section Section => Section.Removed;
	}
}