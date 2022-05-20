using System.Threading.Tasks;
using Changey.Options;
using Changey.Services;

namespace Changey.Commands;

internal class YankCommand : ICommand
{
	public YankCommand(YankOption option, IVersionYanker versionYanker)
	{
		_option = option;
		_versionYanker = versionYanker;
	}

	public async Task Execute()
	{
		await _versionYanker.Yank(_option.Path);
	}

	private readonly YankOption _option;
	private readonly IVersionYanker _versionYanker;
}