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

		[Value(0, Required = true, HelpText = "The message that should be added to the section", MetaName = nameof(Message))]
		public string Message { get; }

		internal abstract Section Section { get; }
	}
}