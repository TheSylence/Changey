using System.Collections.Generic;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Changey.Models
{
	internal class ChangeLog
	{
		public bool UsesSemVer { get; set; }

		public IList<Version> Versions { get; set; } = new List<Version>();
	}
}