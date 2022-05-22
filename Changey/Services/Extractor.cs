using System;
using System.Linq;
using System.Threading.Tasks;
using Changey.Models;

namespace Changey.Services;

internal class Extractor : IExtractor
{
	public Extractor(ILogger logger, IChangeLogSerializer changeLogSerializer)
	{
		_logger = logger;
		_changeLogSerializer = changeLogSerializer;
	}

	public async Task<bool> Extract(string source, string version, string target, bool header)
	{
		ChangeLog changeLog;
		try
		{
			changeLog = await _changeLogSerializer.Deserialize(source);
		}
		catch (Exception ex)
		{
			_logger.Error($"Failed to read changelog from {source}", ex);
			return false;
		}

		var versionToSerialize = changeLog.Versions.FirstOrDefault(v => v.Name == version);
		if (versionToSerialize == null)
		{
			_logger.Error($"Version '{version}' not found in changelog at '{source}'");
			return false;
		}

		try
		{
			await _changeLogSerializer.Serialize(versionToSerialize, target, header);
		}
		catch (Exception ex)
		{
			_logger.Error($"Failed to extract changelog to {target}", ex);
			return false;
		}

		return true;
	}

	private readonly ILogger _logger;
	private readonly IChangeLogSerializer _changeLogSerializer;
}