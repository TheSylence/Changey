using System;
using System.Threading.Tasks;
using Changey.Commands;
using Changey.Services;
using NSubstitute;
using Xunit;

namespace Changey.Tests.Commands
{
	public class InitCommandTests
	{
		[Fact]
		public async Task ExecuteShouldCallChangeLogCreator()
		{
			// Arrange
			var changeLogCreator = Substitute.For<IChangeLogCreator>();
			var sut = new InitCommand(true, "file.name", false, false, changeLogCreator);

			// Act
			await sut.Execute();

			// Assert
			await changeLogCreator.Received(1).CreateChangelog("file.name", true);
		}

		[Fact]
		public async Task ExecuteShouldLogWhenCreatorThrows()
		{
			// Arrange

			var changeLogCreator = Substitute.For<IChangeLogCreator>();
			changeLogCreator.CreateChangelog(Arg.Any<string>(), Arg.Any<bool>())
				.Returns(Task.FromException(new Exception("test-exception")));

			var sut = new InitCommand(true, "file.name", false, false, changeLogCreator);

			var logger = Substitute.For<ILogger>();
			sut.InjectLogger(logger);

			// Act
			await sut.Execute();

			// Assert
			logger.Received(1).Error(Arg.Any<string>(), Arg.Is<Exception>(e => e.Message.Contains("test-exception")));
		}
	}
}