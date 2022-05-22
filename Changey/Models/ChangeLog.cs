using System.Collections.Generic;
using Changey.Services;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Changey.Models;

internal class ChangeLog
{
    public TemplateSet UrlTemplates { get; set; } = new("", "", "");
    public bool UsesSemVer { get; set; }
    public IList<Version> Versions { get; set; } = new List<Version>();
}