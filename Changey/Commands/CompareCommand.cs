using System;
using System.Threading.Tasks;
using Changey.Options;
using Changey.Services;

namespace Changey.Commands;

internal class CompareCommand : ICommand
{
	public CompareCommand(CompareOption option, ICompareGenerator compareGenerator)
	{
		_option = option;
		_compareGenerator = compareGenerator;
	}

	public async Task Execute()
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

	private readonly CompareOption _option;
	private readonly ICompareGenerator _compareGenerator;
}