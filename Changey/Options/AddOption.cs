using System.Collections.Generic;
using Changey.Models;
using CommandLine;
using CommandLine.Text;
using JetBrains.Annotations;

namespace Changey.Options
{
	[Verb("add", HelpText = "Adds a new change to the 'Added' section of the current unreleased version")]
	internal class AddOption : SectionOption
	{
		public AddOption(string message, string path, bool silent, bool verbose)
			: base(message, path, silent, verbose)
		{
		}

		[Usage(ApplicationAlias = "changey")]
		[UsedImplicitly]
		public static IEnumerable<Example> Examples => ExampleBuilder.ExamplesFor<AddOption>();

		internal override Section Section => Section.Added;
	}
}