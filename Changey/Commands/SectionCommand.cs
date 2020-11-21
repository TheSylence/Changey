using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Changey.Models;
using Changey.Services;
using CommandLine;

namespace Changey.Commands
{
	internal abstract class SectionCommand : BaseCommand
	{
		[ExcludeFromCodeCoverage]
		protected SectionCommand(string message, bool verbose, bool silent, string path)
			: this(message, verbose, silent, path, null)
		{
		}

		protected SectionCommand(string message, bool verbose, bool silent, string path,
			ISectionAdder? sectionAdder)
			: base(verbose, silent, path)
		{
			Message = message;

			_sectionAdder = sectionAdder ??
			                new SectionAdder(new ChangeLogSerializer(new FileAccess()), Logger);
		}

		[Option('m', HelpText = "The message that should be added to the section", Required = true)]
		public string Message { get; }

		protected abstract Section Section { get; }

		public override async Task Execute()
		{
			await _sectionAdder.AddToSection(Path, Section, Message);
		}

		private readonly ISectionAdder _sectionAdder;
	}
}