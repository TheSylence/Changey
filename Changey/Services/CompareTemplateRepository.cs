using System;
using System.Collections.Generic;

namespace Changey.Services;

internal class CompareTemplateRepository
{
    public CompareTemplateRepository()
    {
        _hostMap = new Dictionary<string, Host>
        {
            { "github.com", Host.Github },
            { "gitlab.com", Host.Gitlab }
        };

        var hubAndLabTemplates = new TemplateSet("%URL%/releases/tag/%VERSION%",
            "%URL%/compare/%OLD_VERSION%...%NEW_VERSION%", "");
        _templates = new Dictionary<Host, TemplateSet>
        {
            { Host.Github, hubAndLabTemplates },
            { Host.Gitlab, hubAndLabTemplates }
        };
    }

    public TemplateSet? TemplateFor(string baseUrl)
    {
        var host = DetermineHost(baseUrl);

        return _templates.TryGetValue(host, out var template)
            ? template with { Base = baseUrl }
            : null;
    }

    private Host DetermineHost(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            if (!url.StartsWith("https://"))
                url = "https://" + url;
            
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                return Host.None;
        }

        return _hostMap.TryGetValue(uri.Host, out var host)
            ? host
            : Host.None;
    }

    private readonly Dictionary<string, Host> _hostMap;
    private readonly Dictionary<Host, TemplateSet> _templates;
}