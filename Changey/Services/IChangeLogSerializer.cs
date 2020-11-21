using System.Threading.Tasks;
using Changey.Models;

namespace Changey.Services
{
	internal interface IChangeLogSerializer
	{
		Task Serialize(ChangeLog changeLog, string path);
		Task<ChangeLog> Deserialize(string path);
	}
}