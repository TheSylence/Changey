using System.Threading.Tasks;
using Changey.Options;
using Changey.Services;

namespace Changey.Commands
{
	internal class ExtractCommand : ICommand
	{
		public ExtractCommand(ExtractOption option, IExtractor extractor)
		{
			_option = option;
			_extractor = extractor;
		}

		public async Task Execute()
		{
			await _extractor.Extract(_option.Path, _option.Version, _option.Target, _option.Header);
		}

		private readonly ExtractOption _option;
		private readonly IExtractor _extractor;
	}
}