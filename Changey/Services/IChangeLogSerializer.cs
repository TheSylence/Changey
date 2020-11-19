using System.Threading.Tasks;
using Changey.Models;

namespace Changey.Services
{
	internal interface IChangeLogSerializer
	{
		string Serialize(ChangeLog changeLog);
		Task<ChangeLog> Deserialize(string path);
	}
}