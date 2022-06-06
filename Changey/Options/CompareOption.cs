using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using JetBrains.Annotations;

namespace Changey.Options;

[Verb("compare",
	HelpText =
		"Generate compare URLs for all version found in the changelog. This will overwrite any previous compare URLs.")]
internal class CompareOption : BaseOption
{
	public CompareOption(string baseUrl, string compareTemplate, string releaseTemplate, string path, bool silent,
		bool verbose)
		: base(path, silent, verbose)
	{
		BaseUrl = baseUrl;
		CompareTemplate = compareTemplate;
		ReleaseTemplate = releaseTemplate;
	}

	[Option('b', HelpText = CompareOptionTexts.BaseUrl, Required = true)]
	public string BaseUrl { get; }

	[Option('c', HelpText = CompareOptionTexts.CompareTemplate)]
	public string CompareTemplate { get; }

	[Usage(ApplicationAlias = "changey")]
	[UsedImplicitly]
	public static IEnumerable<Example> Examples
	{
		get
		{
			yield return new Example("Generate compare links for a project hosted on github.",
				new CompareOption("github.com/org/repo", string.Empty, string.Empty, string.Empty, false, false));

			yield return new Example("Generate compare links for a project hosted on gitlab.",
				new CompareOption("gitlab.com/org/repo", string.Empty, string.Empty, string.Empty, false, false));

			yield return new Example("Specify templates when project is hosted elsewhere.",
				new CompareOption("example.com/git", "%URL%/compare/%NEW_VERSION%/%OLD_VERSION%",
					"%URL%/releases/%VERSION%", string.Empty, false, false));
		}
	}

	[Option('r', HelpText = CompareOptionTexts.ReleaseTemplate)]
	public string ReleaseTemplate { get; }
}