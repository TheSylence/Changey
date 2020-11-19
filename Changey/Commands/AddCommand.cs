using System.Diagnostics.CodeAnalysis;
using Changey.Models;
using Changey.Services;
using CommandLine;

namespace Changey.Commands
{
	[Verb("add", HelpText = "Adds a new change to the 'Added' section of the current unreleased version")]
	internal class AddCommand : SectionCommand
	{
		[ExcludeFromCodeCoverage]
		public AddCommand(string message, bool verbose, bool silent, string path)
			: base(message, verbose, silent, path)
		{
		}

		public AddCommand(string message, bool verbose, bool silent, string path, ISectionAdder? sectionAdder)
			: base(message, verbose, silent, path, sectionAdder)
		{
		}

		protected override Section Section => Section.Added;
	}
}