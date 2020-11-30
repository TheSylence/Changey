using Changey.Models;
using CommandLine;

namespace Changey.Options
{
	internal abstract class SectionOption : BaseOption
	{
		protected SectionOption(string message, string path, bool silent, bool verbose)
			: base(path, silent, verbose)
		{
			Message = message;
		}

		[Option('m', HelpText = "The message that should be added to the section", Required = true)]
		public string Message { get; }

		internal abstract Section Section { get; }
	}
}