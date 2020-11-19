using System.Diagnostics.CodeAnalysis;
using Changey.Models;
using Changey.Services;
using CommandLine;

namespace Changey.Commands
{
	[Verb("change", HelpText = "Adds a new change to the 'Changed' section of the current unreleased version")]
	internal class ChangeCommand : SectionCommand
	{
		[ExcludeFromCodeCoverage]
		public ChangeCommand(string message, bool verbose, bool silent, string path)
			: base(message, verbose, silent, path)
		{
		}

		public ChangeCommand(string message, bool verbose, bool silent, string path, ISectionAdder? sectionAdder)
			: base(message, verbose, silent, path, sectionAdder)
		{
		}

		protected override Section Section => Section.Changed;
	}
}