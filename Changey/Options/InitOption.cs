﻿using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using JetBrains.Annotations;

namespace Changey.Options;

[Verb("init", HelpText = "Create a new changelog")]
internal class InitOption : BaseOption
{
	public InitOption(string baseUrl, string compareTemplate, bool overwrite, string releaseTemplate, bool semVer, string path, bool silent, bool verbose)
		: base(path, silent, verbose)
	{
		BaseUrl = baseUrl;
		CompareTemplate = compareTemplate;
		SemVer = semVer;
		Overwrite = overwrite;
		ReleaseTemplate = releaseTemplate;
	}

	[Option('b', HelpText = CompareOptionTexts.BaseUrl)]
	public string BaseUrl { get; }

	[Option('c', HelpText = CompareOptionTexts.CompareTemplate)]
	public string CompareTemplate { get; }
	
	[Usage(ApplicationAlias = "changey")]
	[UsedImplicitly]
	public static IEnumerable<Example> Examples
	{
		get
		{
			yield return new Example("Initialize an new changelog at ./changelog.md",
				new InitOption("", "", false, "", false, string.Empty, false, false));
			yield return new Example("Initialize a new changelog at ./changes.md",
				new InitOption("", "", false, "", false, "changes.md", false, false));
			yield return new Example("Initialize a changelog at ./changes.md and overwrite if it already exists",
				new InitOption("", "", true, "", false, "changes.md", false, false));
			yield return new Example("Initialize a new changelog but omit semver compliance information",
				new InitOption("", "", false, "", true, string.Empty, false, false));
		}
	}

	[Option('o', HelpText = "Indicates whether an existing changelog should be overwritten", Default = false)]
	public bool Overwrite { get; }
	
	[Option('r', HelpText = CompareOptionTexts.ReleaseTemplate)]
	public string ReleaseTemplate { get; }

	[Option(HelpText = "Indicates the project uses semver", Default = true)]
	public bool SemVer { get; }
}