using System;
using System.Threading.Tasks;
using Changey.Options;
using Changey.Services;

namespace Changey.Commands;

internal class InitCommand : ICommand
{
	public InitCommand(InitOption option, IChangeLogCreator changeLogCreator, ICompareGenerator compareGenerator)
	{
		_option = option;
		_changeLogCreator = changeLogCreator;
		_compareGenerator = compareGenerator;
	}

	public async Task Execute()
	{
		await ExecuteInit();

		if (!string.IsNullOrEmpty(_option.BaseUrl))
			await ExecuteCompare();
	}

	private async Task ExecuteCompare()
	{
		try
		{
			await _compareGenerator.Generate(_option.Path, _option.BaseUrl, _option.CompareTemplate,
				_option.ReleaseTemplate);
		}
		catch (Exception ex)
		{
			_option.Logger.Error("Failed to set up compare URLs", ex);
		}
	}

	private async Task ExecuteInit()
	{
		try
		{
			_option.Logger.Verbose($"Creating changelog at '{_option.Path}'");
			await _changeLogCreator.CreateChangelog(_option.Path, _option.SemVer, _option.Overwrite);
			_option.Logger.Verbose($"Created changelog at '{_option.Path}'");
		}
		catch (Exception ex)
		{
			_option.Logger.Error("Failed to create changelog", ex);
		}
	}

	private readonly InitOption _option;
	private readonly IChangeLogCreator _changeLogCreator;
	private readonly ICompareGenerator _compareGenerator;
}