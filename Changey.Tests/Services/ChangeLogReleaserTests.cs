using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Changey.Models;
using Changey.Services;
using NSubstitute;
using Xunit;
using Version = Changey.Models.Version;

namespace Changey.Tests.Services;

public class ChangeLogReleaserTests
{
	private static void AddUnreleasedVersionTo(ChangeLog existingChangeLog)
	{
		existingChangeLog.Versions.Add(new Version
		{
			ReleaseDate = null
		});
	}

	private static ChangeLogReleaser GenerateSut(ChangeLog existingChangeLog,
		IChangeLogSerializer? changeLogSerializer = null, ICompareGenerator? compareGenerator = null)
	{
		var logger = Substitute.For<ILogger>();
		if (changeLogSerializer == null)
		{
			changeLogSerializer = Substitute.For<IChangeLogSerializer>();
			changeLogSerializer.Deserialize(Arg.Any<string>()).Returns(existingChangeLog);
		}

		compareGenerator ??= Substitute.For<ICompareGenerator>();
		var sut = new ChangeLogReleaser(logger, changeLogSerializer, compareGenerator);
		return sut;
	}

	[Fact]
	public async Task ReleaseShouldFailWhenNoUnreleasedVersionIsPresent()
	{
		// Arrange
		var existingChangeLog = new ChangeLog();
		existingChangeLog.Versions.Add(new Version
		{
			ReleaseDate = DateTime.Now
		});

		var sut = GenerateSut(existingChangeLog);
		var fileName = Path.GetTempFileName();

		// Act
		var actual = await sut.Release(fileName, null, "1.2", false);

		// Assert
		Assert.False(actual);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public async Task ReleaseShouldNotReleaseOlderVersionUnlessForced(bool force)
	{
		// Arrange
		var existingChangeLog = new ChangeLog();
		existingChangeLog.Versions.Add(new Version
		{
			ReleaseDate = DateTime.Now,
			Name = "1.0.0"
		});
		AddUnreleasedVersionTo(existingChangeLog);

		const string version = "0.9.0";
		var fileName = Path.GetTempFileName();
		var sut = GenerateSut(existingChangeLog);

		// Act
		var actual = await sut.Release(fileName, null, version, force);

		// Assert
		Assert.Equal(force, actual);
	}

	[Fact]
	public async Task ReleaseShouldNotThrowWhenSerializerThrows()
	{
		// Arrange
		var existingChangeLog = new ChangeLog();
		AddUnreleasedVersionTo(existingChangeLog);

		var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
		changeLogSerializer.Deserialize(Arg.Any<string>()).Returns(Task.FromResult(existingChangeLog));
		changeLogSerializer.Serialize(Arg.Any<ChangeLog>(), Arg.Any<string>())
			.Returns(Task.FromException(new Exception("test-exception")));
		var sut = GenerateSut(existingChangeLog, changeLogSerializer);

		// Act
		var ex = await Record.ExceptionAsync(async () => await sut.Release("", null, "1", false));

		// Assert
		Assert.Null(ex);
	}

	[Fact]
	public async Task ReleaseShouldUpdateCompareUrls()
	{
		// Arrange
		var existingChangeLog = new ChangeLog
		{
			UrlTemplates = new TemplateSet("release", "compare", "base")
		};
		AddUnreleasedVersionTo(existingChangeLog);

		const string version = "0.9.0";
		var fileName = Path.GetTempFileName();

		var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
		changeLogSerializer.Deserialize(Arg.Any<string>()).Returns(existingChangeLog);
		var compareGenerator = Substitute.For<ICompareGenerator>();
		compareGenerator.Generate(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
			.Returns(true);

		var sut = GenerateSut(existingChangeLog, changeLogSerializer, compareGenerator);

		// Act
		var actual = await sut.Release(fileName, null, version, false);

		// Assert
		Assert.True(actual);
		await compareGenerator.ReceivedWithAnyArgs(1).Generate("", "", "", "");
	}

	[Fact]
	public async Task ReleaseShouldUpdateDateWhenGiven()
	{
		// Arrange
		var existingChangeLog = new ChangeLog();
		AddUnreleasedVersionTo(existingChangeLog);

		var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
		changeLogSerializer.Deserialize(Arg.Any<string>()).Returns(existingChangeLog);
		var sut = GenerateSut(existingChangeLog, changeLogSerializer);

		var fileName = Path.GetTempFileName();
		const string version = "1.2.3";
		const string date = "2020-12-02";

		// Act
		var actual = await sut.Release(fileName, date, version, false);

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
		AddUnreleasedVersionTo(existingChangeLog);

		var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
		changeLogSerializer.Deserialize(Arg.Any<string>()).Returns(existingChangeLog);
		var sut = GenerateSut(existingChangeLog, changeLogSerializer);

		var fileName = Path.GetTempFileName();
		const string version = "1.2.3";

		// Act
		var actual = await sut.Release(fileName, null, version, false);

		// Assert
		Assert.True(actual);

		await changeLogSerializer.Received(1).Serialize(Arg.Is<ChangeLog>(x =>
				x.Versions.First().ReleaseDate!.Value.Date == DateTime.Now.Date && x.Versions.First().Name == "1.2.3"),
			fileName);
	}
}