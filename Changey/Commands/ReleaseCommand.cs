using System;
using System.Threading.Tasks;
using Changey.Options;
using Changey.Services;

namespace Changey.Commands
{
	internal class ReleaseCommand : ICommand
	{
		public ReleaseCommand(ReleaseOption option, IChangeLogReleaser changeLogReleaser)
		{
			_option = option;
			_changeLogReleaser = changeLogReleaser;
		}

		public async Task Execute()
		{
			try
			{
				if (!await _changeLogReleaser.Release(_option.Path, _option.Date, _option.Name, _option.Force))
					_option.Logger.Warning("Could not release version");
				else
					_option.Logger.Verbose("Released version");
			}
			catch (Exception exception)
			{
				_option.Logger.Error("Failed to release version", exception);
			}
		}

		private readonly ReleaseOption _option;
		private readonly IChangeLogReleaser _changeLogReleaser;
	}
}