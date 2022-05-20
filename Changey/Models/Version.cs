using System;
using System.Collections.Generic;

// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Changey.Models;

internal class Version
{
    public IList<Change> Added { get; set; } = new List<Change>();
    public IList<Change> Changed { get; set; } = new List<Change>();
    public string CompareUrl { get; set; } = string.Empty;
    public IList<Change> Deprecated { get; set; } = new List<Change>();
    public IList<Change> Fixed { get; set; } = new List<Change>();
    public string Name { get; set; } = string.Empty;
    public DateTime? ReleaseDate { get; set; }
    public IList<Change> Removed { get; set; } = new List<Change>();
    public IList<Change> Security { get; set; } = new List<Change>();
    public bool Yanked { get; set; }
}