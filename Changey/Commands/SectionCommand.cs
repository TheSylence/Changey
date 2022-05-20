using System.Threading.Tasks;
using Changey.Options;
using Changey.Services;

namespace Changey.Commands;

internal class SectionCommand : ICommand
{
	public SectionCommand(SectionOption option, ISectionAdder sectionAdder)
	{
		_option = option;
		_sectionAdder = sectionAdder;
	}

	public async Task Execute()
	{
		await _sectionAdder.AddToSection(_option.Path, _option.Section, _option.Message);
	}

	private readonly SectionOption _option;
	private readonly ISectionAdder _sectionAdder;
}