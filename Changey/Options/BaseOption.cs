using System;
using CommandLine;
using JetBrains.Annotations;

namespace Changey.Options;

[PublicAPI]
internal abstract class BaseOption
{
	protected BaseOption(string path, bool silent, bool verbose)
	{
		Verbose = verbose;
		Silent = silent;
		Path = path;

		Logger = new Logger(Console.Out, Silent, Verbose);
	}

	internal ILogger Logger { get; private set; }

	[Option('p', HelpText = "Path to the changelog file that will be created", Default = "changelog.md")]
	public string Path { get; }

	[Option('s', Default = false, HelpText = "Disable log output completely")]
	public bool Silent { get; }

	[Option('v', Default = false, HelpText = "Enable verbose log output")]
	public bool Verbose { get; }

	internal void InjectLogger(ILogger logger)
	{
		Logger = logger;
	}
}