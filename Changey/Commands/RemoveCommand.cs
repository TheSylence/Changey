using System.Diagnostics.CodeAnalysis;
using Changey.Models;
using Changey.Services;
using CommandLine;

namespace Changey.Commands
{
	[Verb("remove", HelpText = "Adds a new change to the 'Removed' section of the current unreleased version")]
	internal class RemoveCommand : SectionCommand
	{
		[ExcludeFromCodeCoverage]
		public RemoveCommand(string message, bool verbose, bool silent, string path)
			: base(message, verbose, silent, path)
		{
		}

		public RemoveCommand(string message, bool verbose, bool silent, string path, ISectionAdder? sectionAdder)
			: base(message, verbose, silent, path, sectionAdder)
		{
		}

		protected override Section Section => Section.Removed;
	}
}