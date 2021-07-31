using System.Threading.Tasks;

namespace Changey.Services
{
	internal interface IChangeLogReleaser
	{
		Task<bool> Release(string path, string? date, string version, bool forceRelease);
	}
}