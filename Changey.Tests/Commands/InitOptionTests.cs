using System;
using System.Threading.Tasks;
using Changey.Commands;
using Changey.Options;
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
			var option = new InitOption(true, true, "file.name", false, false);
			option.InjectLogger(Substitute.For<ILogger>());
			var sut = new InitCommand(option, changeLogCreator);

			// Act
			await sut.Execute();

			// Assert
			await changeLogCreator.Received(1).CreateChangelog("file.name", true, true);
		}

		[Fact]
		public async Task ExecuteShouldLogWhenCreatorThrows()
		{
			// Arrange
			var changeLogCreator = Substitute.For<IChangeLogCreator>();
			changeLogCreator.CreateChangelog(Arg.Any<string>(), Arg.Any<bool>(), true)
				.Returns(Task.FromException(new Exception("test-exception")));

			var option = new InitOption(true, true, "file.name", false, false);

			var logger = Substitute.For<ILogger>();
			option.InjectLogger(logger);

			var sut = new InitCommand(option, changeLogCreator);

			// Act
			await sut.Execute();

			// Assert
			logger.Received(1).Error(Arg.Any<string>(), Arg.Is<Exception>(e => e.Message.Contains("test-exception")));
		}
	}
}