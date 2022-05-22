using System;
using System.Linq;
using System.Threading.Tasks;
using Changey.Models;
using Version = Changey.Models.Version;

namespace Changey.Services;

internal class CompareGenerator : ICompareGenerator
{
    public CompareGenerator(ILogger logger, IChangeLogSerializer changeLogSerializer)
    {
        _logger = logger;
        _changeLogSerializer = changeLogSerializer;
    }

    public async Task<bool> Generate(string path, string baseUrl, string compareTemplate, string releaseTemplate)
    {
        _logger.Verbose($"Generating compare URLs with {baseUrl}");

        var template = _templateRepository.TemplateFor(baseUrl);
        if (template == null)
        {
            _logger.Verbose("Unknown host in base url. Using provided template values");

            if (string.IsNullOrEmpty(compareTemplate))
            {
                _logger.Error("No compareTemplate given. Aborting.");
                return false;
            }

            if (string.IsNullOrEmpty(releaseTemplate))
            {
                _logger.Error("No releaseTemplate given. Aborting.");
                return false;
            }

            template = new TemplateSet(releaseTemplate, compareTemplate, baseUrl);
        }

        var changeLog = await _changeLogSerializer.Deserialize(path);

        if (GenerateUrls(changeLog, template))
        {
            try
            {
                await _changeLogSerializer.Serialize(changeLog, path);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to write changelog to {path}", ex);
                return false;
            }
        }

        return true;
    }

    private static string GenerateCompareUrl(Version latest, Version previous, TemplateSet template) => template.Compare
        .Replace("%URL%", template.Base)
        .Replace("%NEW_VERSION%", latest.Name)
        .Replace("%OLD_VERSION%", previous.Name);

    private static string GenerateLatestCompareUrl(Version latest, TemplateSet template) => template.Compare
        .Replace("%URL%", template.Base)
        .Replace("%NEW_VERSION%", "HEAD")
        .Replace("%OLD_VERSION%", latest.Name);

    private static string GenerateReleaseUrl(Version version, TemplateSet template) => template.Release
        .Replace("%URL%", template.Base)
        .Replace("%VERSION%", version.Name);

    private bool GenerateUrls(ChangeLog changeLog, TemplateSet templateSet)
    {
        changeLog.UrlTemplates = templateSet;
        var allVersions = changeLog.Versions.OrderByDescending(v => v.ReleaseDate ?? DateTime.MaxValue).ToList();

        if (!allVersions.Any())
        {
            _logger.Info("No versions found in changelog. Not generating any compare URLs");
            return false;
        }

        for (var i = 0; i < allVersions.Count - 1; ++i)
        {
            var newVersion = allVersions[i];
            var previousVersion = allVersions[i + 1];

            if (newVersion.ReleaseDate == null)
                newVersion.CompareUrl = GenerateLatestCompareUrl(previousVersion, templateSet);
            else
                newVersion.CompareUrl = GenerateCompareUrl(newVersion, previousVersion, templateSet);
        }

        if (allVersions[^1].ReleaseDate != null)
            allVersions[^1].CompareUrl = GenerateReleaseUrl(allVersions[^1], templateSet);
        else
            allVersions[^1].CompareUrl = string.Empty;

        return true;
    }

    private readonly ILogger _logger;
    private readonly IChangeLogSerializer _changeLogSerializer;
    private readonly CompareTemplateRepository _templateRepository = new();
}