using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Changey.Models;
using Changey.Services;
using NSubstitute;
using Xunit;
using Version = Changey.Models.Version;

namespace Changey.Tests.Services;

public class ExtractorTests
{
	[Fact]
	public async Task ExtractShouldFailWhenChangelogCannotBeRead()
	{
		// Arrange
		var logger = Substitute.For<ILogger>();
		var serializer = Substitute.For<IChangeLogSerializer>();
		serializer.Deserialize(Arg.Any<string>())
			.Returns(Task.FromException<ChangeLog>(new Exception("test-exception")));

		var sut = new Extractor(logger, serializer);

		// Act
		var actual = await sut.Extract("", "", "", false);

		// Assert
		Assert.False(actual);
	}

	[Fact]
	public async Task ExtractShouldFailWhenSerializationFails()
	{
		// Arrange
		var logger = Substitute.For<ILogger>();
		var serializer = Substitute.For<IChangeLogSerializer>();
		var changeLog = new ChangeLog
		{
			Versions = new List<Version>
			{
				new()
				{
					Name = "1.0"
				}
			}
		};

		serializer.Deserialize(Arg.Any<string>()).Returns(Task.FromResult(changeLog));
		serializer.Serialize(Arg.Any<Version>(), Arg.Any<string>(), false)
			.Returns(Task.FromException(new Exception("test-exception")));

		var sut = new Extractor(logger, serializer);

		// Act
		var actual = await sut.Extract("", "1.0", "", false);

		// Assert
		Assert.False(actual);
	}

	[Fact]
	public async Task ExtractShouldFailWhenVersionWasNotFound()
	{
		// Arrange
		var logger = Substitute.For<ILogger>();
		var serializer = Substitute.For<IChangeLogSerializer>();
		serializer.Deserialize(Arg.Any<string>()).Returns(Task.FromResult(new ChangeLog()));

		var sut = new Extractor(logger, serializer);

		// Act
		var actual = await sut.Extract("", "", "", false);

		// Assert
		Assert.False(actual);
	}

	[Fact]
	public async Task ExtractShouldSerializeCorrectVersion()
	{
		// Arrange
		var logger = Substitute.For<ILogger>();
		var serializer = Substitute.For<IChangeLogSerializer>();
		var changeLog = new ChangeLog
		{
			Versions = new List<Version>
			{
				new()
				{
					Name = "1.0"
				},
				new()
				{
					Name = "0.9"
				}
			}
		};

		serializer.Deserialize(Arg.Any<string>()).Returns(Task.FromResult(changeLog));
		serializer.Serialize(Arg.Any<Version>(), Arg.Any<string>(), false).Returns(Task.CompletedTask);

		var sut = new Extractor(logger, serializer);

		// Act
		var actual = await sut.Extract("", "1.0", "", false);

		// Assert
		Assert.True(actual);
	}
}