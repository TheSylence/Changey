using System;
using System.Threading.Tasks;
using CommandLine;

namespace Changey.Commands
{
	internal abstract class BaseCommand
	{
		protected BaseCommand(bool verbose, bool silent, string path)
		{
			Verbose = verbose;
			Silent = silent;
			Path = path;

			Logger = new Logger(Console.Out, Silent, Verbose);
		}

		[Option('p', HelpText = "Path to the changelog file that will be created", Default = "changelog.md")]
		public string Path { get; }

		[Option('s', Default = false, HelpText = "Disable log output completely")]
		public bool Silent { get; }

		[Option('v', Default = false, HelpText = "Enable verbose log output")]
		public bool Verbose { get; }

		protected ILogger Logger { get; private set; }

		public abstract Task Execute();

		internal void InjectLogger(ILogger logger)
		{
			Logger = logger;
		}
	}
}