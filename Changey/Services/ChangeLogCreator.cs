using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Changey.Models;
using Version = Changey.Models.Version;

namespace Changey.Services
{
	internal class ChangeLogCreator
		: IChangeLogCreator
	{
		public ChangeLogCreator(ILogger logger, IChangeLogSerializer changeLogSerializer)
		{
			_logger = logger;
			_changeLogSerializer = changeLogSerializer;
		}

		public async Task CreateChangelog(string path, bool usesSemver, bool overwrite)
		{
			if (File.Exists(path))
			{
				if (overwrite)
					_logger.Verbose($"Overwriting changelog at '{path}'");
				else
				{
					_logger.Error($"Changelog at '{path}' already exists");
					return;
				}
			}

			var changelog = new ChangeLog
			{
				UsesSemVer = usesSemver,
				Versions = new List<Version>
				{
					new()
				}
			};

			try
			{
				await _changeLogSerializer.Serialize(changelog, path);
			}
			catch (Exception ex)
			{
				_logger.Error("Failed to create file", ex);
			}
		}

		private readonly ILogger _logger;
		private readonly IChangeLogSerializer _changeLogSerializer;
	}
}