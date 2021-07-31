using System.Threading.Tasks;
using Changey.Services;

namespace Changey.Tests.Services
{
	internal class MockFileAccess : IFileAccess
	{
		public string ContentRead { get; set; } = string.Empty;
		public string ContentWritten { get; private set; } = string.Empty;

		public Task<string> ReadFromFile(string path) => Task.FromResult(ContentRead);

		public Task WriteToFile(string path, string content)
		{
			ContentWritten = content;
			return Task.CompletedTask;
		}
	}
}