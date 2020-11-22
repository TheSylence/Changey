using Changey.Models;
using CommandLine;

namespace Changey.Options
{
	[Verb("remove", HelpText = "Adds a new change to the 'Removed' section of the current unreleased version")]
	internal class RemoveOption : SectionOption
	{
		public RemoveOption(string message, bool verbose, bool silent, string path)
			: base(message, verbose, silent, path)
		{
		}

		public override Section Section => Section.Removed;
	}
}