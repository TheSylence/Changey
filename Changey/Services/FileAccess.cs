using System.IO;
using System.Threading.Tasks;

namespace Changey.Services
{
	internal class FileAccess : IFileAccess
	{
		public async Task<string> ReadFromFile(string path) => await File.ReadAllTextAsync(path);

		public async Task WriteToFile(string path, string content) => await File.WriteAllTextAsync(path, content);
	}
}