using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Changey.Models;
using Changey.Services;
using Xunit;
using FileAccess = Changey.Services.FileAccess;
using Version = Changey.Models.Version;

namespace Changey.Tests.Services;

public class ChangeLogSerializationTests
{
	[Fact]
	public async Task SerializedChangeLogShouldBeDeserializable()
	{
		// Arrange
		var expected = new ChangeLog
		{
			// UsesSemVer = true,
			Versions = new List<Version>
			{
				new()
				{
					Added = new List<Change>
					{
						new() { Text = "add" }
					}
				},
				new()
				{
					ReleaseDate = new DateTime(2019, 1, 1),
					Name = "1.2",
					Changed = new List<Change>
					{
						new() { Text = "change" }
					}
				}
			}
		};

		const string fileName = "file.name";
		var sut = new ChangeLogSerializer(new FileAccess());

		// Act
		await sut.Serialize(expected, fileName);
		var actual = await sut.Deserialize(fileName);

		// Assert
		Assert.NotNull(actual);

		Assert.Equal(expected.UsesSemVer, actual.UsesSemVer);

		Assert.Equal(expected.Versions.Count, actual.Versions.Count);
		Assert.Contains(actual.Versions, v => v.ReleaseDate == null);
		Assert.Contains(actual.Versions, v => v.ReleaseDate != null);
		Assert.Contains(actual.Versions, v => v.Name == "1.2");

		var unreleasedVersion = actual.Versions.Single(v => v.ReleaseDate == null);
		Assert.Collection(unreleasedVersion.Added, x => Assert.Equal("add", x.Text));

		var releasedVersion = actual.Versions.Single(v => v.ReleaseDate != null);
		Assert.Collection(releasedVersion.Changed, x => Assert.Equal("change", x.Text));
	}

	[Fact]
	public async Task SerializeShouldContainEntireChanges()
	{
		// Arrange
		var changeLog = new ChangeLog
		{
			Versions = new List<Version>
			{
				new()
				{
					ReleaseDate = new DateTime(2019, 4, 26),
					Name = "1.2.3",
					Added = new List<Change>
					{
						new() { Text = "AddA" },
						new() { Text = "AddB" },
						new() { Text = "AddC" }
					},
					Deprecated = new List<Change>
					{
						new() { Text = "DeprecatedA" },
						new() { Text = "DeprecatedB" },
						new() { Text = "DeprecatedC" }
					},
					Fixed = new List<Change>
					{
						new() { Text = "FixedA" },
						new() { Text = "FixedB" },
						new() { Text = "FixedC" }
					},
					Removed = new List<Change>
					{
						new() { Text = "RemovedA" },
						new() { Text = "RemovedB" },
						new() { Text = "RemovedC" }
					},
					Security = new List<Change>
					{
						new() { Text = "SecurityA" },
						new() { Text = "SecurityB" },
						new() { Text = "SecurityC" }
					},
					Changed = new List<Change>
					{
						new() { Text = "ChangedA" },
						new() { Text = "ChangedB" },
						new() { Text = "ChangedC" }
					}
				}
			}
		};

		var fileName = Path.GetTempFileName();
		var fileAccess = new MockFileAccess();
		var sut = new ChangeLogSerializer(fileAccess);

		// Act
		await sut.Serialize(changeLog, fileName);

		// Assert
		var actual = fileAccess.ContentWritten;
		Assert.Contains("### Added", actual);
		Assert.Contains("### Changed", actual);
		Assert.Contains("### Deprecated", actual);
		Assert.Contains("### Removed", actual);
		Assert.Contains("### Fixed", actual);
		Assert.Contains("### Security", actual);

		Assert.Contains("AddA", actual);
		Assert.Contains("AddB", actual);
		Assert.Contains("AddC", actual);

		Assert.Contains("ChangedA", actual);
		Assert.Contains("ChangedB", actual);
		Assert.Contains("ChangedC", actual);

		Assert.Contains("DeprecatedA", actual);
		Assert.Contains("DeprecatedB", actual);
		Assert.Contains("DeprecatedC", actual);

		Assert.Contains("RemovedA", actual);
		Assert.Contains("RemovedB", actual);
		Assert.Contains("RemovedC", actual);

		Assert.Contains("FixedA", actual);
		Assert.Contains("FixedB", actual);
		Assert.Contains("FixedC", actual);

		Assert.Contains("SecurityA", actual);
		Assert.Contains("SecurityB", actual);
		Assert.Contains("SecurityC", actual);
	}

	[Fact]
	public async Task SerializeShouldContainReleasedDateAndVersionName()
	{
		// Arrange
		var changeLog = new ChangeLog
		{
			Versions = new List<Version>
			{
				new()
				{
					ReleaseDate = new DateTime(2019, 4, 26),
					Name = "1.2.3"
				}
			}
		};

		var fileAccess = new MockFileAccess();
		var sut = new ChangeLogSerializer(fileAccess);

		// Act
		await sut.Serialize(changeLog, "");

		// Assert
		var actual = fileAccess.ContentWritten;
		Assert.Contains("[1.2.3]", actual);
		Assert.Contains("2019-04-26", actual);
	}

	[Fact]
	public async Task SerializeShouldContainUnreleasedVersion()
	{
		// Arrange
		var changeLog = new ChangeLog
		{
			Versions = new List<Version>
			{
				new()
				{
					ReleaseDate = null
				}
			}
		};

		var fileAccess = new MockFileAccess();
		var sut = new ChangeLogSerializer(fileAccess);

		// Act
		await sut.Serialize(changeLog, "");

		// Assert
		var actual = fileAccess.ContentWritten;
		Assert.Contains("[Unreleased]", actual);
	}

	[Fact]
	public async Task SerializeShouldCreateChangeLogWithCorrectName()
	{
		// Arrange
		var changeLog = new ChangeLog
		{
			UsesSemVer = true
		};

		var fileAccess = new MockFileAccess();
		var sut = new ChangeLogSerializer(fileAccess);

		// Act
		await sut.Serialize(changeLog, "");

		// Assert
		var actual = fileAccess.ContentWritten;
		Assert.NotEmpty(actual);
		Assert.Contains("keepachangelog.com", actual);
		Assert.Contains("semver.org", actual);
	}

	[Fact]
	public async Task SerializeShouldIndicateYankedReleases()
	{
		// Arrange
		var changeLog = new ChangeLog
		{
			Versions = new List<Version>
			{
				new()
				{
					ReleaseDate = new DateTime(2019, 4, 26),
					Name = "1.2.3",
					Yanked = true
				}
			}
		};

		var fileAccess = new MockFileAccess();
		var sut = new ChangeLogSerializer(fileAccess);

		// Act
		await sut.Serialize(changeLog, "");

		// Assert
		var actual = fileAccess.ContentWritten;
		Assert.Contains("[YANKED]", actual);
	}

	[Fact]
	public async Task SerializeShouldNotWriteSemVerInfoWhenDisabled()
	{
		// Arrange
		var changeLog = new ChangeLog
		{
			UsesSemVer = false
		};

		var fileAccess = new MockFileAccess();
		var sut = new ChangeLogSerializer(fileAccess);

		// Act
		await sut.Serialize(changeLog, "");

		// Assert
		var actual = fileAccess.ContentWritten;
		Assert.NotEmpty(actual);
		Assert.DoesNotContain("semver.org", actual);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public async Task SerializeShouldOnlyIncludeHeaderIfSpecified(bool header)
	{
		// Arrange
		var version = new Version
		{
			Name = "1.2.3",
			ReleaseDate = new DateTime(2021, 2, 3),
			Added = new List<Change>
			{
				new() { Text = "Add Message" }
			},
			Fixed = new List<Change>
			{
				new() { Text = "Fix Message" }
			}
		};

		var fileAccess = new MockFileAccess();
		var sut = new ChangeLogSerializer(fileAccess);

		// Act
		await sut.Serialize(version, "", header);

		// Assert
		var lines = fileAccess.ContentWritten.Split(Environment.NewLine);
		Assert.Contains(lines, l => l.Contains("Add Message"));
		Assert.Contains(lines, l => l.Contains("Fix Message"));
		if (header)
			Assert.Contains(lines, l => l.Contains("1.2.3"));
		else
			Assert.DoesNotContain(lines, l => l.Contains("1.2.3"));
	}

	[Fact]
	public async Task SerializeShouldWriteCompareUrlsWhenHavingMultipleVersion()
	{
		// Arrange
		var changeLog = new ChangeLog
		{
			Versions = new List<Version>
			{
				new()
				{
					ReleaseDate = new DateTime(2019, 4, 26),
					Name = "1.2.3",
					CompareUrl = "http://example.com/1.2.3..1.2.2"
				},
				new()
				{
					ReleaseDate = new DateTime(2019, 4, 25),
					Name = "1.2.2",
					CompareUrl = "http://example.com/1.2.2"
				}
			}
		};

		var fileAccess = new MockFileAccess();
		var sut = new ChangeLogSerializer(fileAccess);

		// Act
		await sut.Serialize(changeLog, "");

		// Assert
		var actual = fileAccess.ContentWritten;
		Assert.Contains("[1.2.3]: http://example.com/1.2.3..1.2.2", actual);
		Assert.Contains("[1.2.2]: http://example.com/1.2.2", actual);
	}

	[Fact]
	public async Task SerializeShouldWriteCompareUrlsWhenHavingSingleVersion()
	{
		// Arrange
		var changeLog = new ChangeLog
		{
			Versions = new List<Version>
			{
				new()
				{
					ReleaseDate = new DateTime(2019, 4, 26),
					Name = "1.2.3",
					CompareUrl = "http://example.com/1.2.3"
				}
			}
		};

		var fileAccess = new MockFileAccess();
		var sut = new ChangeLogSerializer(fileAccess);

		// Act
		await sut.Serialize(changeLog, "");

		// Assert
		var actual = fileAccess.ContentWritten;
		Assert.Contains("[1.2.3]: http://example.com/1.2.3", actual);
	}

	[Fact]
	public async Task SerializeShouldWriteCompareUrlTemplates()
	{
		// Arrange
		var changeLog = new ChangeLog
		{
			UrlTemplates = new TemplateSet("%ReleaseTemplate%",
				"%CompareTemplate%", "baseurl")
		};

		var fileAccess = new MockFileAccess();
		var sut = new ChangeLogSerializer(fileAccess);

		// Act
		await sut.Serialize(changeLog, "");

		// Assert
		var actual = fileAccess.ContentWritten;
		Assert.Contains("<!-- Release: %ReleaseTemplate% -->", actual);
		Assert.Contains("<!-- Compare: %CompareTemplate% -->", actual);
		Assert.Contains("<!-- BaseUrl: baseurl -->", actual);
	}

	[Fact]
	public async Task UnreleasedVersionShouldBeWrittenToTopOfChangelog()
	{
		// Arrange
		var changeLog = new ChangeLog
		{
			Versions = new List<Version>
			{
				new()
				{
					ReleaseDate = new DateTime(2021, 1, 2),
					Name = "1.1"
				},
				new()
				{
					ReleaseDate = null
				},
				new()
				{
					ReleaseDate = new DateTime(2020, 12, 20),
					Name = "1.0"
				}
			}
		};

		var fileAccess = new MockFileAccess();
		var sut = new ChangeLogSerializer(fileAccess);

		// Act
		await sut.Serialize(changeLog, "");

		// Assert
		var actual = fileAccess.ContentWritten;

		var indexOfOneZero = actual.IndexOf("[1.0]", StringComparison.Ordinal);
		var indexOfOneOne = actual.IndexOf("[1.1]", StringComparison.Ordinal);
		var indexOfUnreleased = actual.IndexOf("[Unreleased]", StringComparison.Ordinal);

		Assert.NotEqual(-1, indexOfOneOne);
		Assert.NotEqual(-1, indexOfOneZero);
		Assert.NotEqual(-1, indexOfUnreleased);

		Assert.True(indexOfUnreleased < indexOfOneOne);
		Assert.True(indexOfOneOne < indexOfOneZero);
	}
}