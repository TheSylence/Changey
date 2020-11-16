using System.Threading.Tasks;

namespace Changey.Models
{
	internal interface IChangeLogSerializer
	{
		string Serialize(ChangeLog changeLog);
		Task<ChangeLog> Deserialize(string path);
	}
}