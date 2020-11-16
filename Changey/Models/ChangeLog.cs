using System.Collections.Generic;

namespace Changey.Models
{
	internal class ChangeLog
	{
		public bool UsesSemVer { get; set; }

		public IList<Version> Versions { get; set; } = new List<Version>();
	}
}