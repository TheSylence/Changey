using System.IO;
using System.Threading.Tasks;
using Changey.Models;
using Changey.Services;
using NSubstitute;
using Xunit;

namespace Changey.Tests.Services
{
	public class ChangeLogCreatorTests
	{
		[Fact]
		public async Task CreateChangeLogShouldNotThrowWhenFileCantBeWritten()
		{
			// Arrange
			var fileName = "!invalid?file:name";

			var logger = Substitute.For<ILogger>();
			var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
			const string changelogContent = "changelog-content";
			changeLogSerializer.Serialize(Arg.Any<ChangeLog>()).Returns(changelogContent);
			var sut = new ChangeLogCreator(logger, changeLogSerializer);

			// Act
			var ex = await Record.ExceptionAsync(async () => await sut.CreateChangelog(fileName, true));

			// Assert
			Assert.Null(ex);
		}

		[Fact]
		public async Task CreateChangeLogShouldWriteContentToFile()
		{
			// Arrange
			var fileName = Path.GetTempFileName();

			var logger = Substitute.For<ILogger>();
			var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
			const string changelogContent = "changelog-content";
			changeLogSerializer.Serialize(Arg.Any<ChangeLog>()).Returns(changelogContent);
			var sut = new ChangeLogCreator(logger, changeLogSerializer);

			// Act
			await sut.CreateChangelog(fileName, true);

			// Assert
			Assert.True(File.Exists(fileName));
			var fileContent = await File.ReadAllTextAsync(fileName);
			File.Delete(fileName);

			Assert.Equal(changelogContent, fileContent);
		}
	}
}