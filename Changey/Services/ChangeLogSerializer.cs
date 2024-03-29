﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Changey.Models;
using Version = Changey.Models.Version;

namespace Changey.Services;

internal class ChangeLogSerializer : IChangeLogSerializer
{
	public ChangeLogSerializer(IFileAccess fileAccess)
	{
		_fileAccess = fileAccess;
	}

	public async Task<ChangeLog> Deserialize(string path)
	{
		var content = await _fileAccess.ReadFromFile(path);

		var changeLog = new ChangeLog();

		var lines = content.Split(Environment.NewLine);
		var offset = FindHeader(lines);
		if (offset < 0)
			return changeLog;

		offset = ParseHeader(lines, offset, changeLog);
		if (offset < 0)
			return changeLog;

		offset = ParseVersions(lines, offset, changeLog);
		if (offset < 0)
			return changeLog;

		ParseCompareLinks(lines, offset, changeLog);
		ParseVariables(lines, changeLog);

		return changeLog;
	}

	public async Task Serialize(ChangeLog changeLog, string path)
	{
		var content = Serialize(changeLog);
		await _fileAccess.WriteToFile(path, content);
	}

	public async Task Serialize(Version version, string path, bool header)
	{
		var sb = new StringBuilder();
		WriteVersion(sb, version, header);
		await _fileAccess.WriteToFile(path, sb.ToString());
	}

	private static void AddChange(Version version, string sectionName, string changeText)
	{
		var listToAdd = sectionName switch
		{
			AddedSection => version.Added,
			ChangedSection => version.Changed,
			DeprecatedSection => version.Deprecated,
			FixedSection => version.Fixed,
			RemovedSection => version.Removed,
			SecuritySection => version.Security,
			_ => null
		};

		listToAdd?.Add(new Change { Text = changeText });
	}

	private static int FindCompareLinks(IList<string> lines, int offset)
	{
		for (var i = offset; i < lines.Count; ++i)
		{
			if (lines[i].StartsWith('['))
				return i;
		}

		return -1;
	}

	private static int FindHeader(IList<string> lines)
	{
		for (var i = 0; i < lines.Count; ++i)
		{
			if (lines[i].StartsWith("# Changelog"))
				return i;
		}

		return -1;
	}

	private static int FindNextChangeSection(IList<string> lines, int offset, int nextVersionOffset)
	{
		for (var i = offset; i < nextVersionOffset; ++i)
		{
			if (lines[i].StartsWith("### "))
				return i;
		}

		return -1;
	}

	private static int FindVersion(IList<string> lines, int offset)
	{
		for (var i = offset; i < lines.Count; ++i)
		{
			if (lines[i].StartsWith("## "))
				return i;
		}

		return -1;
	}

	private static bool IsVariable(string line)
	{
		var trimmed = line.Trim();
		return trimmed.StartsWith("<!-- ") && trimmed.EndsWith("-->");
	}

	private static void ParseCompareLink(string line, ChangeLog changeLog)
	{
		var parts = line.Split(':', 2);
		if (parts.Length != 2)
			return;

		var versionName = parts[0].Trim('[', ']');
		var version = changeLog.Versions.FirstOrDefault(v => v.Name == versionName);
		if (version != null)
			version.CompareUrl = parts[1].Trim();
	}

	private static void ParseCompareLinks(IList<string> lines, int offset, ChangeLog changeLog)
	{
		for (var i = offset; i < lines.Count; ++i)
		{
			var line = lines[i];
			ParseCompareLink(line, changeLog);
		}
	}

	private static int ParseHeader(IList<string> lines, int offset, ChangeLog changeLog)
	{
		var keepAChangeLogLine = lines[offset + 3];

		if (!keepAChangeLogLine.Contains("keepachangelog.com"))
			return -1;

		offset += 4;
		var semVerLine = lines[offset];
		if (string.IsNullOrEmpty(semVerLine))
			return offset;

		if (semVerLine.Contains("semver.org"))
			changeLog.UsesSemVer = true;

		offset++;
		return offset;
	}

	private static int ParseSection(IList<string> lines, int offset, Version version)
	{
		var sb = new StringBuilder();
		var sectionName = lines[offset].TrimStart('#').Trim();

		for (var i = offset + 1; i < lines.Count; ++i)
		{
			if (string.IsNullOrWhiteSpace(lines[i]))
			{
				if (sb.Length <= 0)
					return i;
				
				AddChange(version, sectionName, sb.ToString());
				sb.Clear();
				return i;
			}

			if (lines[i].StartsWith('-'))
			{
				if (sb.Length > 0)
				{
					AddChange(version, sectionName, sb.ToString());
					sb.Clear();
				}

				sb.Append(lines[i].TrimStart('-').Trim());
			}
			else if (lines[i].StartsWith("  "))
			{
				sb.AppendLine();
				sb.Append(lines[i].TrimStart('-').Trim());
			}
		}

		return -1;
	}

	private static void ParseTemplate(string line, ChangeLog changeLog)
	{
		line = line.Replace("<!--", "").Replace("-->", "").Trim();
		var parts = line.Split(':', 2);
		var template = parts[1].Trim();

		changeLog.UrlTemplates = parts[0].Trim() switch
		{
			"Release" => changeLog.UrlTemplates with { Release = template },
			"Compare" => changeLog.UrlTemplates with { Compare = template },
			"BaseUrl" => changeLog.UrlTemplates with { Base = template },
			_ => changeLog.UrlTemplates
		};
	}

	private static void ParseVariables(IEnumerable<string> lines, ChangeLog changeLog)
	{
		foreach (var line in lines.Where(IsVariable))
		{
			ParseTemplate(line, changeLog);
		}
	}

	private static int ParseVersion(IList<string> lines, int offset, ChangeLog changeLog)
	{
		offset = FindVersion(lines, offset);
		if (offset < 0)
			return offset;

		var match = VersionPattern.Match(lines[offset]);
		if (!match.Success)
			return offset;

		var version = new Version
		{
			Name = match.Groups[1].Value
		};

		if (!string.IsNullOrEmpty(match.Groups[2].Value))
		{
			version.ReleaseDate =
				DateTime.ParseExact(match.Groups[2].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
		}

		if (!string.IsNullOrEmpty(match.Groups[3].Value))
			version.Yanked = true;

		changeLog.Versions.Add(version);

		var nextVersionOffset = FindVersion(lines, offset + 1);
		var compareLinksStart = FindCompareLinks(lines, offset + 1);
		nextVersionOffset = nextVersionOffset < 0
			? compareLinksStart < 0
				? lines.Count
				: compareLinksStart
			: nextVersionOffset;

		var nextSectionOffset = offset;
		while ((nextSectionOffset = FindNextChangeSection(lines, nextSectionOffset, nextVersionOffset)) >= 0)
		{
			nextSectionOffset = ParseSection(lines, nextSectionOffset, version);
			if (nextSectionOffset < 0)
				break;
		}

		offset = nextVersionOffset - 1;

		return offset;
	}

	private static int ParseVersions(IList<string> lines, int offset, ChangeLog changeLog)
	{
		var nextOffset = offset;
		for (var i = offset; i < lines.Count; ++i)
		{
			var newOffset = ParseVersion(lines, i, changeLog);
			if (newOffset < 0)
				break;

			i = newOffset;
			nextOffset = newOffset;
		}

		return nextOffset;
	}

	private static string Serialize(ChangeLog changeLog)
	{
		var sb = new StringBuilder();

		WriteHeader(changeLog, sb);

		var versions = changeLog.Versions.OrderByDescending(v => v.ReleaseDate ?? DateTime.MaxValue).ToList();
		foreach (var version in versions)
		{
			WriteVersion(sb, version, true);
		}

		sb.AppendLine();
		foreach (var version in versions)
		{
			WriteCompareLink(sb, version);
		}

		WriteVariables(sb, changeLog);

		return sb.ToString();
	}

	private static string TitleForSection(Section section)
	{
		return section switch
		{
			Section.Added => AddedSection,
			Section.Changed => ChangedSection,
			Section.Deprecated => DeprecatedSection,
			Section.Fixed => FixedSection,
			Section.Removed => RemovedSection,
			Section.Security => SecuritySection,
			_ => string.Empty
		};
	}

	private static void WriteChanges(StringBuilder sb, Section section, IList<Change> changes)
	{
		if (!changes.Any())
			return;

		var title = TitleForSection(section);
		sb.AppendLine($"### {title}");
		foreach (var change in changes)
		{
			sb.AppendLine($"- {change.Text}");
		}

		sb.AppendLine();
	}

	private static void WriteCompareLink(StringBuilder sb, Version version)
	{
		if (!string.IsNullOrEmpty(version.CompareUrl))
			sb.AppendLine($"[{version.Name}]: {version.CompareUrl}");
	}

	private static void WriteHeader(ChangeLog changeLog, StringBuilder sb)
	{
		sb.AppendLine("# Changelog");
		sb.AppendLine("All notable changes to this project will be documented in this file.");
		sb.AppendLine();
		sb.Append("The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)");
		if (changeLog.UsesSemVer)
		{
			sb.AppendLine(",");
			sb.AppendLine("and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).");
		}
		else
			sb.AppendLine(".");

		sb.AppendLine();
	}

	private static void WriteUrlTemplates(StringBuilder sb, ChangeLog changeLog)
	{
		if (!string.IsNullOrEmpty(changeLog.UrlTemplates.Release))
			sb.AppendLine($"<!-- Release: {changeLog.UrlTemplates.Release} -->");
		if (!string.IsNullOrEmpty(changeLog.UrlTemplates.Compare))
			sb.AppendLine($"<!-- Compare: {changeLog.UrlTemplates.Compare} -->");
		if (!string.IsNullOrEmpty(changeLog.UrlTemplates.Base))
			sb.AppendLine($"<!-- BaseUrl: {changeLog.UrlTemplates.Base} -->");
	}

	private static void WriteVariables(StringBuilder sb, ChangeLog changeLog)
	{
		WriteUrlTemplates(sb, changeLog);
	}

	private static void WriteVersion(StringBuilder sb, Version version, bool includeHeader)
	{
		if (includeHeader)
		{
			sb.Append("## ");
			sb.Append(version.ReleaseDate.HasValue
				? $"[{version.Name}] - {version.ReleaseDate:yyyy-MM-dd}"
				: "[Unreleased]");

			if (version.Yanked)
				sb.Append(" [YANKED]");

			sb.AppendLine();
		}

		WriteChanges(sb, Section.Added, version.Added);
		WriteChanges(sb, Section.Changed, version.Changed);
		WriteChanges(sb, Section.Deprecated, version.Deprecated);
		WriteChanges(sb, Section.Removed, version.Removed);
		WriteChanges(sb, Section.Fixed, version.Fixed);
		WriteChanges(sb, Section.Security, version.Security);
	}

	private readonly IFileAccess _fileAccess;

	private const string AddedSection = "Added";
	private const string ChangedSection = "Changed";
	private const string DeprecatedSection = "Deprecated";
	private const string FixedSection = "Fixed";
	private const string RemovedSection = "Removed";
	private const string SecuritySection = "Security";

	private static readonly Regex VersionPattern =
		new("\\[([\\w\\.]+)\\](?: - (\\d{4}-\\d{2}-\\d{2})( \\[YANKED\\])?)?", RegexOptions.Compiled);
}