using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Changey.Services
{
	internal class ChangeLogReleaser : IChangeLogReleaser
	{
		public ChangeLogReleaser(ILogger logger, IChangeLogSerializer changeLogSerializer)
		{
			_logger = logger;
			_changeLogSerializer = changeLogSerializer;
		}

		public async Task<bool> Release(string path, string? date, string version, bool forceRelease)
		{
			var releaseDate = DetermineDate(date);
			_logger.Verbose($"Releasing version {version} on {releaseDate.Date} in {path}");

			var changeLog = await _changeLogSerializer.Deserialize(path);

			foreach (var changeLogVersion in changeLog.Versions)
			{
				_logger.Verbose($"Existing version: {changeLogVersion.Name}");
			}

			var unreleased = changeLog.Versions.FirstOrDefault(v => v.ReleaseDate == null);
			if (unreleased == null)
			{
				_logger.Error("No unreleased version found in changelog");
				return false;
			}
			
			if (Version.TryParse(version, out var versionToRelease) && !forceRelease)
			{
				var releasedVersions = changeLog.Versions.Where(v => v.ReleaseDate != null).ToList();
				
				var badVersions = releasedVersions.Where(v => !Version.TryParse(v.Name, out _));
				if (!badVersions.Any())
				{
					var newerVersions = releasedVersions.Where(v => Version.Parse(v.Name) >= versionToRelease).ToList();
					if (newerVersions.Any())
					{
						_logger.Warning("You are trying to release a version that is older than already released version(s)");
						foreach (var newerVersion in newerVersions)
						{
							_logger.Warning($"Version '{newerVersion.Name}' is newer that version to be released '{versionToRelease}'");
						}

						_logger.Warning("Specify -f flag to force releasing this version anyways.");
						return false;
					}
				}
			}

			unreleased.ReleaseDate = releaseDate;
			unreleased.Name = version;

			try
			{
				await _changeLogSerializer.Serialize(changeLog, path);
			}
			catch (Exception ex)
			{
				_logger.Error("Failed to yank version", ex);
				return false;
			}

			return true;
		}

		private static DateTime DetermineDate(string? date) => string.IsNullOrEmpty(date)
			? DateTime.Now
			: DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

		private readonly ILogger _logger;
		private readonly IChangeLogSerializer _changeLogSerializer;
	}
}