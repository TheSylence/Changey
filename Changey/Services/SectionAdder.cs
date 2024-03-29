﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Changey.Models;
using Version = Changey.Models.Version;

namespace Changey.Services;

internal class SectionAdder : ISectionAdder
{
	public SectionAdder(IChangeLogSerializer changeLogSerializer, ILogger logger)
	{
		_changeLogSerializer = changeLogSerializer;
		_logger = logger;
	}

	public async Task AddToSection(string path, Section section, string message)
	{
		_logger.Verbose($"Adding new '{section}' to '{path}'");

		ChangeLog changeLog;
		try
		{
			changeLog = await _changeLogSerializer.Deserialize(path);
		}
		catch (Exception ex)
		{
			_logger.Error($"Failed to read file '{path}'", ex);
			return;
		}

		if (!AddToSection(changeLog, section, message))
		{
			_logger.Verbose($"Could not add new '{section}' to '{path}'");
			return;
		}

		try
		{
			await _changeLogSerializer.Serialize(changeLog, path);
		}
		catch (Exception ex)
		{
			_logger.Error("Failed to create file", ex);
		}
	}

	private bool AddToSection(ChangeLog changeLog, Section section, string message)
	{
		var version = changeLog.Versions.FirstOrDefault(v => v.ReleaseDate == null);
		if (version == null)
		{
			version = new Version();
			changeLog.Versions.Add(version);
		}

		var list = section switch
		{
			Section.Added => version.Added,
			Section.Changed => version.Changed,
			Section.Deprecated => version.Deprecated,
			Section.Fixed => version.Fixed,
			Section.Removed => version.Removed,
			Section.Security => version.Security,
			_ => null
		};

		if (list == null)
		{
			_logger.Error($"Unknown section: {section}");
			return false;
		}

		list.Add(new Change { Text = message });
		return true;
	}

	private readonly IChangeLogSerializer _changeLogSerializer;
	private readonly ILogger _logger;
}