using System.IO;
using System.Threading.Tasks;
using Xunit;
using FileAccess = Changey.Services.FileAccess;

namespace Changey.Tests.Services
{
	public class FileAccessTests
	{
		[Fact]
		public async Task ReadShouldReadAllTextFromFile()
		{
			// Arrange
			var sut = new FileAccess();
			var fileName = Path.GetTempFileName();
			await File.WriteAllTextAsync(fileName, "content");

			// Act
			var actual = await sut.ReadFromFile(fileName);

			// Assert
			Assert.Equal("content", actual);
		}

		[Fact]
		public async Task WriteShouldWriteAllTextToFile()
		{
			// Arrange
			var sut = new FileAccess();
			var fileName = Path.GetTempFileName();

			// Act
			await sut.WriteToFile(fileName, "content");

			// Assert
			var actual = await File.ReadAllTextAsync(fileName);
			Assert.Equal("content", actual);
		}
	}
}