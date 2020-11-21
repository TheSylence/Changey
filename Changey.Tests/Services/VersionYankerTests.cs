using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Changey.Models;
using Changey.Services;
using NSubstitute;
using Xunit;
using Version = Changey.Models.Version;

namespace Changey.Tests.Services
{
	public class VersionYankerTests
	{
		[Fact]
		public async Task YankShouldLogErrorWhenNoReleasedVersionIsFound()
		{
			// Arrange
			var changeLog = new ChangeLog
			{
				Versions = new List<Version>
				{
					new Version()
				}
			};

			var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
			changeLogSerializer.Deserialize("").Returns(Task.FromResult(changeLog));

			var logger = Substitute.For<ILogger>();
			var sut = new VersionYanker(logger, changeLogSerializer);

			// Act
			var actual = await sut.Yank("");

			// Assert
			Assert.False(actual);
			logger.Received(1).Error(Arg.Any<string>());
		}

		[Fact]
		public async Task YankShouldNotThrowsWhenSerializerThrowsDuringSerialization()
		{
			// Arrange
			var changeLog = new ChangeLog
			{
				Versions = new List<Version>
				{
					new Version
					{
						ReleaseDate = DateTime.Now
					}
				}
			};

			var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
			changeLogSerializer.Deserialize("").Returns(Task.FromResult(changeLog));
			changeLogSerializer.Serialize(Arg.Any<ChangeLog>(), "")
				.Returns(Task.FromException(new Exception("test-exception")));

			var logger = Substitute.For<ILogger>();
			var sut = new VersionYanker(logger, changeLogSerializer);

			// Act
			var ex = await Record.ExceptionAsync(async () => await sut.Yank(""));

			// Assert
			Assert.Null(ex);
		}

		[Fact]
		public async Task YankShouldNotThrowWhenSerializerThrowsDuringDeserialization()
		{
			// Arrange
			var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
			changeLogSerializer.Deserialize("")
				.Returns(Task.FromException<ChangeLog>(new Exception("test-exception")));

			var logger = Substitute.For<ILogger>();
			var sut = new VersionYanker(logger, changeLogSerializer);

			// Act
			var ex = await Record.ExceptionAsync(async () => await sut.Yank(""));

			// Assert
			Assert.Null(ex);
		}

		[Fact]
		public async Task YankShouldYankVersion()
		{
			// Arrange
			var changeLog = new ChangeLog
			{
				Versions = new List<Version>
				{
					new Version
					{
						ReleaseDate = DateTime.Now
					}
				}
			};

			var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
			changeLogSerializer.Deserialize("").Returns(Task.FromResult(changeLog));

			var logger = Substitute.For<ILogger>();
			var sut = new VersionYanker(logger, changeLogSerializer);

			// Act
			var actual = await sut.Yank("");

			// Assert
			Assert.True(actual);
			await changeLogSerializer.Received(1).Serialize(Arg.Is<ChangeLog>(c => c.Versions.First().Yanked), "");
		}
	}
}