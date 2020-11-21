﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Changey.Models;

namespace Changey.Services
{
	internal interface IVersionYanker
	{
		Task<bool> Yank(string path);
	}

	internal class VersionYanker : IVersionYanker
	{
		public VersionYanker(ILogger logger, IChangeLogSerializer changeLogSerializer)
		{
			_logger = logger;
			_changeLogSerializer = changeLogSerializer;
		}

		public async Task<bool> Yank(string path)
		{
			ChangeLog changeLog;
			try
			{
				changeLog = await _changeLogSerializer.Deserialize(path);
			}
			catch (Exception ex)
			{
				_logger.Error($"Failed to read changelog from {path}", ex);
				return false;
			}

			var latestVersion = changeLog.Versions.Where(v => v.ReleaseDate.HasValue)
				.OrderByDescending(v => v.ReleaseDate!.Value).FirstOrDefault();

			if (latestVersion == null)
			{
				_logger.Error("Found no released version to yank");
				return false;
			}

			latestVersion.Yanked = true;
			try
			{
				await _changeLogSerializer.Serialize(changeLog, path);
			}
			catch (Exception ex)
			{
				_logger.Error($"Failed to write changelog to {path}", ex);
				return false;
			}

			return true;
		}

		private readonly ILogger _logger;
		private readonly IChangeLogSerializer _changeLogSerializer;
	}
}