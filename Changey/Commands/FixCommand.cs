using System.Diagnostics.CodeAnalysis;
using Changey.Models;
using Changey.Services;
using CommandLine;

namespace Changey.Commands
{
	[Verb("fix", HelpText = "Adds a new change to the 'Fixed' section of the current unreleased version")]
	internal class FixCommand : SectionCommand
	{
		[ExcludeFromCodeCoverage]
		public FixCommand(string message, bool verbose, bool silent, string path)
			: base(message, verbose, silent, path)
		{
		}

		public FixCommand(string message, bool verbose, bool silent, string path, ISectionAdder? sectionAdder)
			: base(message, verbose, silent, path, sectionAdder)
		{
		}

		protected override Section Section => Section.Fixed;
	}
}