using System.Collections.Generic;
using Changey.Models;
using CommandLine;
using CommandLine.Text;

namespace Changey.Options
{
	[Verb("fix", HelpText = "Adds a new change to the 'Fixed' section of the current unreleased version")]
	internal class FixOption : SectionOption
	{
		public FixOption(string message, string path, bool verbose, bool silent)
			: base(message, path, silent, verbose)
		{
		}

		[Usage(ApplicationAlias = "changey")] public static IEnumerable<Example> Examples => ExampleBuilder.ExamplesFor<FixOption>();

		internal override Section Section => Section.Fixed;
	}
}