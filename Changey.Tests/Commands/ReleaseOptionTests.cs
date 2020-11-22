using System;
using System.Threading.Tasks;
using Changey.Commands;
using Changey.Options;
using Changey.Services;
using NSubstitute;
using Xunit;

namespace Changey.Tests.Commands
{
	public class ReleaseCommandTests
	{
		[Fact]
		public async Task ExecuteShouldCallChangeLogReleaser()
		{
			// Arrange
			var changeLogReleaser = Substitute.For<IChangeLogReleaser>();
			changeLogReleaser.Release(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
				.Returns(Task.FromResult(true));
			var option = new ReleaseOption(null, "1.2.3", "file.name", false, false);
			var sut = new ReleaseCommand(option, changeLogReleaser);

			// Act
			await sut.Execute();

			// Assert
			await changeLogReleaser.Received(1).Release("file.name", null, "1.2.3");
		}

		[Fact]
		public async Task ExecuteShouldLogWhenReleaseFails()
		{
			// Arrange
			var changeLogReleaser = Substitute.For<IChangeLogReleaser>();
			changeLogReleaser.Release(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
				.Returns(Task.FromResult(false));

			var option = new ReleaseOption(null, "1.2.3", "file.name", false, false);

			var logger = Substitute.For<ILogger>();
			option.InjectLogger(logger);

			var sut = new ReleaseCommand(option, changeLogReleaser);

			// Act
			await sut.Execute();

			// Assert
			logger.Received(1).Warning(Arg.Any<string>());
		}

		[Fact]
		public async Task ExecuteShouldLogWhenReleaseThrows()
		{
			// Arrange
			var changeLogReleaser = Substitute.For<IChangeLogReleaser>();
			changeLogReleaser.Release(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
				.Returns(Task.FromException<bool>(new Exception("test-exception")));

			var option = new ReleaseOption(null, "1.2.3", "file.name", false, false);

			var logger = Substitute.For<ILogger>();
			option.InjectLogger(logger);

			var sut = new ReleaseCommand(option, changeLogReleaser);


			// Act
			await sut.Execute();

			// Assert
			logger.Received(1).Error(Arg.Any<string>(), Arg.Is<Exception>(e => e.Message.Contains("test-exception")));
		}
	}
}