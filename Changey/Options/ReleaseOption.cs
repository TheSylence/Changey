using CommandLine;

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

		[Value(0, HelpText = "Name of the version to release.", Required = true, MetaName = nameof(Name))]
		public string Name { get; }
	}
}