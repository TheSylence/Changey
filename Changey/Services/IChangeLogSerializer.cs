using System.Threading.Tasks;
using Changey.Models;

namespace Changey.Services
{
	internal interface IChangeLogSerializer
	{
		Task<ChangeLog> Deserialize(string path);
		Task Serialize(ChangeLog changeLog, string path);
		Task Serialize(Version version, string path, bool header);
	}
}