using Changey.Models;
using CommandLine;

namespace Changey.Options
{
	internal abstract class SectionOption : BaseOption
	{
		protected SectionOption(string message, bool verbose, bool silent, string path)
			: base(path, silent, verbose)
		{
			Message = message;
		}

		[Option('m', HelpText = "The message that should be added to the section", Required = true)]
		public string Message { get; }

		public abstract Section Section { get; }
	}
}