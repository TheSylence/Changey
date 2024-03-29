﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Changey.Models;
using Changey.Services;
using NSubstitute;
using Xunit;
using Version = Changey.Models.Version;

namespace Changey.Tests.Services;

public class SectionAdderTests
{
	private static Section SectionFromName(string sectionName) => Enum.Parse<Section>(sectionName);

	private static IEnumerable<Change> SectionList(Version version, Section section)
	{
		return section switch
		{
			Section.Added => version.Added,
			Section.Changed => version.Changed,
			Section.Deprecated => version.Deprecated,
			Section.Fixed => version.Fixed,
			Section.Removed => version.Removed,
			Section.Security => version.Security,
			_ => new List<Change>()
		};
	}

	[Theory]
	[InlineData("Added", "add-message")]
	[InlineData("Removed", "remove-message")]
	[InlineData("Changed", "change-message")]
	[InlineData("Fixed", "fixed-message")]
	[InlineData("Security", "security-message")]
	[InlineData("Deprecated", "deprecated-message")]
	public async Task AddToSectionShouldAddToCorrectSection(string sectionName, string message)
	{
		// Arrange
		var serializer = Substitute.For<IChangeLogSerializer>();
		const string fileName = "path";
		serializer.Deserialize(fileName).Returns(Task.FromResult(new ChangeLog
		{
			Versions = new List<Version>
			{
				new()
			}
		}));

		var logger = Substitute.For<ILogger>();
		var sut = new SectionAdder(serializer, logger);

		var section = SectionFromName(sectionName);

		// Act
		await sut.AddToSection(fileName, section, message);

		// Assert
		await serializer.Received(1).Deserialize(fileName);
		await serializer.Received(1)
			.Serialize(Arg.Is<ChangeLog>(cl =>
				SectionList(cl.Versions.First(), section).Any(a => a.Text == message)), fileName);
	}

	[Fact]
	public async Task AddToSectionShouldFailWhenAddingToUnknownSection()
	{
		// Arrange
		var serializer = Substitute.For<IChangeLogSerializer>();
		const string fileName = "path";
		serializer.Deserialize(fileName).Returns(Task.FromResult(new ChangeLog
		{
			Versions = new List<Version>
			{
				new()
			}
		}));

		var logger = Substitute.For<ILogger>();
		var sut = new SectionAdder(serializer, logger);

		// Act
		await sut.AddToSection(fileName, (Section)int.MaxValue, "test");

		// Assert
		logger.Received(1).Error(Arg.Is<string>(x => x.Contains("section")));
	}

	[Theory]
	[InlineData("Added", "add-message")]
	[InlineData("Removed", "remove-message")]
	[InlineData("Changed", "change-message")]
	[InlineData("Fixed", "fixed-message")]
	[InlineData("Security", "security-message")]
	[InlineData("Deprecated", "deprecated-message")]
	public async Task AddToSectionShouldLog(string sectionName, string message)
	{
		// Arrange
		var serializer = Substitute.For<IChangeLogSerializer>();
		const string fileName = "path";
		serializer.Deserialize(fileName).Returns(Task.FromResult(new ChangeLog
		{
			Versions = new List<Version>
			{
				new()
			}
		}));

		var logger = Substitute.For<ILogger>();
		var sut = new SectionAdder(serializer, logger);

		var section = SectionFromName(sectionName);

		// Act
		await sut.AddToSection(fileName, section, message);

		// Assert
		logger.Received(1).Verbose(Arg.Any<string>());
	}

	[Fact]
	public async Task AddToSectionShouldNotAddWhenDeserializationFails()
	{
		// Arrange
		var logger = Substitute.For<ILogger>();
		var serializer = Substitute.For<IChangeLogSerializer>();
		serializer.Deserialize(Arg.Any<string>())
			.Returns(Task.FromException<ChangeLog>(new Exception("test-exception")));

		var sut = new SectionAdder(serializer, logger);

		// Act
		await sut.AddToSection("path", Section.Added, "test message");

		// Assert
		logger.Received(1).Error(Arg.Any<string>(), Arg.Is<Exception>(e => e.Message == "test-exception"));
		await serializer.DidNotReceive().Serialize(Arg.Any<ChangeLog>(), Arg.Any<string>());
	}

	[Fact]
	public async Task AddToSectionShouldNotFailWhenNoUnreleasedVersionIsPresent()
	{
		// Arrange
		var serializer = Substitute.For<IChangeLogSerializer>();
		const string fileName = "path";
		serializer.Deserialize(fileName).Returns(Task.FromResult(new ChangeLog
		{
			Versions = new List<Version>
			{
				new()
				{
					ReleaseDate = DateTime.Now
				}
			}
		}));

		var logger = Substitute.For<ILogger>();
		var sut = new SectionAdder(serializer, logger);

		// Act
		await sut.AddToSection(fileName, Section.Added, "test");

		// Assert
		logger.DidNotReceive().Error(Arg.Any<string>());
	}

	[Fact]
	public async Task AddToSectionShouldNotThrowWhenFileAccessThrows()
	{
		// Arrange
		var serializer = Substitute.For<IChangeLogSerializer>();
		const string fileName = "path";
		serializer.Deserialize(fileName).Returns(Task.FromResult(new ChangeLog
		{
			Versions = new List<Version>
			{
				new()
			}
		}));

		serializer.Serialize(Arg.Any<ChangeLog>(), Arg.Any<string>())
			.Returns(Task.FromException(new Exception("test-exception")));

		var logger = Substitute.For<ILogger>();
		var sut = new SectionAdder(serializer, logger);

		// Act
		var ex = await Record.ExceptionAsync(async () => await sut.AddToSection(fileName, Section.Added, "message"));

		// Assert
		Assert.Null(ex);
		logger.Received(1).Error(Arg.Any<string>(), Arg.Is<Exception>(e => e.Message.Contains("test-exception")));
	}
}