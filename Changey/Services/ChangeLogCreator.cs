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

		public async Task CreateChangelog(string path, bool usesSemver)
		{
			var changelog = new ChangeLog
			{
				UsesSemVer = usesSemver,
				Versions = new List<Version>
				{
					new Version()
				}
			};

			var content = _changeLogSerializer.Serialize(changelog);

			try
			{
				await File.WriteAllTextAsync(path, content);
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