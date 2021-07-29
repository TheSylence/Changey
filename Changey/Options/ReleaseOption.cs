using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Changey.Options
{
	[Verb("release", HelpText = "Marks the latest unreleased version as released")]
	internal class ReleaseOption : BaseOption
	{
		public ReleaseOption(string? date, string name, string path, bool silent, bool verbose)
			: base(path, silent, verbose)
		{
			Date = date;
			Name = name;
		}

		[Option('d', HelpText = "The date to use for this release. If omitted the current date will be used",
			Default = null)]
		public string? Date { get; }

		[Usage(ApplicationAlias = "changey")]
		public static IEnumerable<Example> Examples
		{
			get
			{
				yield return new Example("Mark the current version as released today with version 1.2.3",
					new ReleaseOption(null, "1.2.3", string.Empty, false, false));
				yield return new Example("Mark the current version as 1.0 released on May 4, 2009",
					new ReleaseOption("2005-05-04", "1.0", string.Empty, false, false));
			}
		}

		[Value(0, HelpText = "Name of the version to release.", Required = true, MetaName = nameof(Name))]
		public string Name { get; }
	}
}