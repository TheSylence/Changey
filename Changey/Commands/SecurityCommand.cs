using System.Diagnostics.CodeAnalysis;
using Changey.Models;
using Changey.Services;
using CommandLine;

namespace Changey.Commands
{
	[Verb("security", HelpText = "Adds a new change to the 'Security' section of the current unreleased version")]
	internal class SecurityCommand : SectionCommand
	{
		[ExcludeFromCodeCoverage]
		public SecurityCommand(string message, bool verbose, bool silent, string path)
			: base(message, verbose, silent, path)
		{
		}

		public SecurityCommand(string message, bool verbose, bool silent, string path, ISectionAdder? sectionAdder)
			: base(message, verbose, silent, path, sectionAdder)
		{
		}

		protected override Section Section => Section.Security;
	}
}