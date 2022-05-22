using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using JetBrains.Annotations;

namespace Changey.Options;

[Verb("extract", HelpText = "Extract changes for a single version from a changelog")]
internal class ExtractOption : BaseOption
{
	public ExtractOption(bool header, string target, string version, string path, bool silent, bool verbose)
		: base(path, silent, verbose)
	{
		Version = version;
		Target = target;
		Header = header;
	}

	[Usage(ApplicationAlias = "changey")]
	[UsedImplicitly]
	public static IEnumerable<Example> Examples
	{
		get
		{
			yield return new Example("Extract all changes for version 1.2.3 and store them in a file called version.md",
				new ExtractOption(false, "version.md", "1.2.3", string.Empty, false, false));
			yield return new Example(
				"Extract all changes for version 1.1 into a file valled 1.1.md and include the version header",
				new ExtractOption(true, "1.1.md", "1.1", string.Empty, false, false));
		}
	}

	[Option('h', HelpText = "Include header from version in extracted file", Default = false)]
	public bool Header { get; }

	[Option('t', HelpText = "Target file to extract changes to", Required = true)]
	public string Target { get; }

	[Value(0, HelpText = "Version to extract changes for", Required = true, MetaName = nameof(Version))]
	public string Version { get; }
}