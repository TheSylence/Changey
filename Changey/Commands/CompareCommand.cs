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
        await _compareGenerator.Generate(_option.Path, _option.BaseUrl, _option.CompareTemplate,
            _option.ReleaseTemplate);
    }

    private readonly CompareOption _option;
    private readonly ICompareGenerator _compareGenerator;
}