using System.Threading.Tasks;

namespace Changey.Services;

internal interface IFileAccess
{
	Task<string> ReadFromFile(string path);
	Task WriteToFile(string path, string content);
}