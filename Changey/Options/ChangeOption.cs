using System.Collections.Generic;
using Changey.Models;
using CommandLine;
using CommandLine.Text;

namespace Changey.Options
{
	[Verb("change", HelpText = "Adds a new change to the 'Changed' section of the current unreleased version")]
	internal class ChangeOption : SectionOption
	{
		public ChangeOption(string message, string path, bool silent, bool verbose)
			: base(message, path, silent, verbose)
		{
		}

		[Usage(ApplicationAlias = "changey")] public static IEnumerable<Example> Examples => ExampleBuilder.ExamplesFor<ChangeOption>();

		internal override Section Section => Section.Changed;
	}
}