using System.Diagnostics.CodeAnalysis;
using Changey.Models;
using Changey.Services;
using CommandLine;

namespace Changey.Commands
{
	[Verb("deprecate", HelpText = "Adds a new change to the 'Deprecated' section of the current unreleased version")]
	internal class DeprecatedCommand : SectionCommand
	{
		[ExcludeFromCodeCoverage]
		public DeprecatedCommand(string message, bool verbose, bool silent, string path)
			: base(message, verbose, silent, path)
		{
		}

		public DeprecatedCommand(string message, bool verbose, bool silent, string path, ISectionAdder? sectionAdder)
			: base(message, verbose, silent, path, sectionAdder)
		{
		}

		protected override Section Section => Section.Deprecated;
	}
}