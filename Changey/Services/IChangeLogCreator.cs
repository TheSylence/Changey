using System.Threading.Tasks;

namespace Changey.Services
{
	internal interface IChangeLogCreator
	{
		Task CreateChangelog(string path, bool usesSemver);
	}
}