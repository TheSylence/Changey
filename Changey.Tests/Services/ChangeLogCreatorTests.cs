using System;
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
		public async Task CreateChangeLogShouldAbortWhenFileExists()
		{
			// Arrange
			var fileName = Path.GetTempFileName();

			var logger = Substitute.For<ILogger>();
			var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
			changeLogSerializer.Serialize(Arg.Any<ChangeLog>(), fileName).Returns(Task.CompletedTask);
			var sut = new ChangeLogCreator(logger, changeLogSerializer);

			// Act
			await sut.CreateChangelog(fileName, true, false);

			// Assert
			await changeLogSerializer.DidNotReceive().Serialize(Arg.Any<ChangeLog>(), fileName);
			logger.Received(1).Error(Arg.Any<string>());
		}

		[Fact]
		public async Task CreateChangeLogShouldNotThrowWhenFileCantBeWritten()
		{
			// Arrange
			const string fileName = "!invalid?file:name";

			var logger = Substitute.For<ILogger>();
			var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
			changeLogSerializer.Serialize(Arg.Any<ChangeLog>(), fileName)
				.Returns(Task.FromException(new Exception("test-exception")));
			var sut = new ChangeLogCreator(logger, changeLogSerializer);

			// Act
			var ex = await Record.ExceptionAsync(async () => await sut.CreateChangelog(fileName, true, true));

			// Assert
			Assert.Null(ex);
		}

		[Fact]
		public async Task CreateChangeLogShouldUseSerializer()
		{
			// Arrange
			var fileName = Path.GetTempFileName();

			var logger = Substitute.For<ILogger>();
			var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
			changeLogSerializer.Serialize(Arg.Any<ChangeLog>(), fileName).Returns(Task.CompletedTask);
			var sut = new ChangeLogCreator(logger, changeLogSerializer);

			// Act
			await sut.CreateChangelog(fileName, true, true);

			// Assert
			await changeLogSerializer.Received(1).Serialize(Arg.Any<ChangeLog>(), fileName);
		}
	}
}