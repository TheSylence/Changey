using System.Collections.Generic;
using Changey.Models;
using CommandLine;
using CommandLine.Text;
using JetBrains.Annotations;

namespace Changey.Options;

[Verb("security", HelpText = "Adds a new change to the 'Security' section of the current unreleased version")]
internal class SecurityOption : SectionOption
{
	public SecurityOption(string message, string path, bool silent, bool verbose)
		: base(message, path, silent, verbose)
	{
	}

	[Usage(ApplicationAlias = "changey")]
	[UsedImplicitly]
	public static IEnumerable<Example> Examples => ExampleBuilder.ExamplesFor<SecurityOption>();
		
	internal override Section Section => Section.Security;
}