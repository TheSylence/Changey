using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Changey.Models;
using Changey.Services;
using NSubstitute;
using Xunit;
using Version = Changey.Models.Version;

namespace Changey.Tests.Services
{
	public class ChangeLogReleaserTests
	{
		[Fact]
		public async Task ReleaseShouldFailWhenNoUnreleasedVersionIsPresent()
		{
			// Arrange
			var existingChangeLog = new ChangeLog();
			existingChangeLog.Versions.Add(new Version
			{
				ReleaseDate = DateTime.Now
			});

			var logger = Substitute.For<ILogger>();
			var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
			changeLogSerializer.Deserialize(Arg.Any<string>()).Returns(existingChangeLog);
			var sut = new ChangeLogReleaser(logger, changeLogSerializer);

			var fileName = Path.GetTempFileName();

			// Act
			var actual = await sut.Release(fileName, null, "1.2");

			// Assert
			Assert.False(actual);
		}

		[Fact]
		public async Task ReleaseShouldNotThrowWhenSerializerThrows()
		{
			// Arrange
			var existingChangeLog = new ChangeLog();
			existingChangeLog.Versions.Add(new Version
			{
				ReleaseDate = null
			});

			var logger = Substitute.For<ILogger>();
			var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
			changeLogSerializer.Deserialize(Arg.Any<string>()).Returns(Task.FromResult(existingChangeLog));
			changeLogSerializer.Serialize(Arg.Any<ChangeLog>(), Arg.Any<string>())
				.Returns(Task.FromException(new Exception("test-exception")));
			var sut = new ChangeLogReleaser(logger, changeLogSerializer);

			// Act
			var ex = await Record.ExceptionAsync(async () => await sut.Release("", null, "1"));

			// Assert
			Assert.Null(ex);
		}

		[Fact]
		public async Task ReleaseShouldUpdateDateWhenGiven()
		{
			// Arrange
			var existingChangeLog = new ChangeLog();
			existingChangeLog.Versions.Add(new Version
			{
				ReleaseDate = null
			});

			var logger = Substitute.For<ILogger>();
			var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
			changeLogSerializer.Deserialize(Arg.Any<string>()).Returns(existingChangeLog);
			var sut = new ChangeLogReleaser(logger, changeLogSerializer);

			var fileName = Path.GetTempFileName();
			const string version = "1.2.3";
			const string date = "2020-12-02";

			// Act
			var actual = await sut.Release(fileName, date, version);

			// Assert
			Assert.True(actual);

			await changeLogSerializer.Received(1).Serialize(Arg.Is<ChangeLog>(x =>
				x.Versions.First().ReleaseDate!.Value.Date == new DateTime(2020, 12, 2)), fileName);
		}

		[Fact]
		public async Task ReleaseShouldUpdateNameAndDate()
		{
			// Arrange
			var existingChangeLog = new ChangeLog();
			existingChangeLog.Versions.Add(new Version
			{
				ReleaseDate = null
			});

			var logger = Substitute.For<ILogger>();
			var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
			changeLogSerializer.Deserialize(Arg.Any<string>()).Returns(existingChangeLog);
			var sut = new ChangeLogReleaser(logger, changeLogSerializer);

			var fileName = Path.GetTempFileName();
			const string version = "1.2.3";

			// Act
			var actual = await sut.Release(fileName, null, version);

			// Assert
			Assert.True(actual);


			await changeLogSerializer.Received(1).Serialize(Arg.Is<ChangeLog>(x =>
					x.Versions.First().ReleaseDate!.Value.Date == DateTime.Now.Date && x.Versions.First().Name == "1.2.3"),
				fileName);
		}
	}
}