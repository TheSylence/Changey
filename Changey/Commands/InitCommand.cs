using System;
using System.Threading.Tasks;
using Changey.Options;
using Changey.Services;

namespace Changey.Commands
{
	internal class InitCommand : ICommand
	{
		public InitCommand(InitOption option, IChangeLogCreator changeLogCreator)
		{
			_option = option;
			_changeLogCreator = changeLogCreator;
		}

		public async Task Execute()
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
	}
}