using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Changey.Models;
using Changey.Services;
using NSubstitute;
using Xunit;
using Version = Changey.Models.Version;

namespace Changey.Tests.Services
{
	public class ChangeLogSerializerTests
	{
		private static StringBuilder GenerateChangeLogHeader()
		{
			var sb = new StringBuilder();
			sb.AppendLine("# Changelog");
			sb.AppendLine("All notable changes to this project will be documented in this file.");
			sb.AppendLine();
			sb.AppendLine("The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),");
			sb.AppendLine("and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).");
			sb.AppendLine();

			return sb;
		}

		[Fact]
		public async Task DeserializeShouldDetermineThatSemVerIsNotUsed()
		{
			// Arrange
			var sb = new StringBuilder();
			sb.AppendLine("# Changelog");
			sb.AppendLine("All notable changes to this project will be documented in this file.");
			sb.AppendLine();
			sb.AppendLine("The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).");
			sb.AppendLine();

			var fileAccess = Substitute.For<IFileAccess>();
			const string fileName = "path";
			fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

			var sut = new ChangeLogSerializer(fileAccess);

			// Act
			var actual = await sut.Deserialize(fileName);

			// Assert
			Assert.NotNull(actual);
			Assert.False(actual.UsesSemVer);
		}

		[Fact]
		public async Task DeserializeShouldDetermineThatSemVerIsUsed()
		{
			// Arrange
			var sb = new StringBuilder();
			sb.AppendLine("# Changelog");
			sb.AppendLine("All notable changes to this project will be documented in this file.");
			sb.AppendLine();
			sb.AppendLine("The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),");
			sb.AppendLine("and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).");
			sb.AppendLine();

			var fileAccess = Substitute.For<IFileAccess>();
			const string fileName = "path";
			fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

			var sut = new ChangeLogSerializer(fileAccess);

			// Act
			var actual = await sut.Deserialize(fileName);

			// Assert
			Assert.NotNull(actual);
			Assert.True(actual.UsesSemVer);
		}

		[Fact]
		public async Task DeserializeShouldHandleEmptySection()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [Unreleased]");
			sb.Append("### Security");

			var fileAccess = Substitute.For<IFileAccess>();
			const string fileName = "path";
			fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

			var sut = new ChangeLogSerializer(fileAccess);

			// Act
			var actual = await sut.Deserialize(fileName);

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Empty(version!.Security);
		}

		[Fact]
		public async Task DeserializeShouldHandleUnreleasedVersion()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [Unreleased]");

			var fileAccess = Substitute.For<IFileAccess>();
			const string fileName = "path";
			fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

			var sut = new ChangeLogSerializer(fileAccess);

			// Act
			var actual = await sut.Deserialize(fileName);

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Null(version!.ReleaseDate);
		}

		[Fact]
		public async Task DeserializeShouldNotParseChangelogWithoutHeader()
		{
			// Arrange
			var sb = new StringBuilder();

			var fileAccess = Substitute.For<IFileAccess>();
			const string fileName = "path";
			fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

			var sut = new ChangeLogSerializer(fileAccess);

			// Act
			var actual = await sut.Deserialize(fileName);

			// Assert
			Assert.NotNull(actual);
			Assert.False(actual.UsesSemVer);
			Assert.Empty(actual.Versions);
		}

		[Fact]
		public async Task DeserializeShouldNotParseChangelogWithoutSpecUrl()
		{
			// Arrange
			var sb = new StringBuilder();
			sb.AppendLine("# Changelog");
			sb.AppendLine("All notable changes to this project will be documented in this file.");
			sb.AppendLine();

			var fileAccess = Substitute.For<IFileAccess>();
			const string fileName = "path";
			fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

			var sut = new ChangeLogSerializer(fileAccess);

			// Act
			var actual = await sut.Deserialize(fileName);

			// Assert
			Assert.NotNull(actual);
			Assert.False(actual.UsesSemVer);
			Assert.Empty(actual.Versions);
		}

		[Fact]
		public async Task DeserializeShouldReadAdded()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [Unreleased]");
			sb.AppendLine("### Added");
			sb.AppendLine("- Change1");
			sb.AppendLine("- Multiline");
			sb.AppendLine("  change");
			sb.AppendLine("- Change3");

			var fileAccess = Substitute.For<IFileAccess>();
			const string fileName = "path";
			fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

			var sut = new ChangeLogSerializer(fileAccess);

			// Act
			var actual = await sut.Deserialize(fileName);

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Collection(version!.Added,
				x => Assert.Equal("Change1", x.Text),
				x => Assert.Equal("Multiline" + Environment.NewLine + "change", x.Text),
				x => Assert.Equal("Change3", x.Text)
			);
		}

		[Fact]
		public async Task DeserializeShouldReadChanged()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [Unreleased]");
			sb.AppendLine("### Changed");
			sb.AppendLine("- Change1");
			sb.AppendLine("- Multiline");
			sb.AppendLine("  change");
			sb.AppendLine("- Change3");

			var fileAccess = Substitute.For<IFileAccess>();
			const string fileName = "path";
			fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

			var sut = new ChangeLogSerializer(fileAccess);

			// Act
			var actual = await sut.Deserialize(fileName);

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Collection(version!.Changed,
				x => Assert.Equal("Change1", x.Text),
				x => Assert.Equal("Multiline" + Environment.NewLine + "change", x.Text),
				x => Assert.Equal("Change3", x.Text)
			);
		}

		[Fact]
		public async Task DeserializeShouldReadDeprecated()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [Unreleased]");
			sb.AppendLine("### Deprecated");
			sb.AppendLine("- Change1");
			sb.AppendLine("- Multiline");
			sb.AppendLine("  change");
			sb.AppendLine("- Change3");

			var fileAccess = Substitute.For<IFileAccess>();
			const string fileName = "path";
			fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

			var sut = new ChangeLogSerializer(fileAccess);

			// Act
			var actual = await sut.Deserialize(fileName);

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Collection(version!.Deprecated,
				x => Assert.Equal("Change1", x.Text),
				x => Assert.Equal("Multiline" + Environment.NewLine + "change", x.Text),
				x => Assert.Equal("Change3", x.Text)
			);
		}

		[Fact]
		public async Task DeserializeShouldReadFixed()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [Unreleased]");
			sb.AppendLine("### Fixed");
			sb.AppendLine("- Change1");
			sb.AppendLine("- Multiline");
			sb.AppendLine("  change");
			sb.AppendLine("- Change3");

			var fileAccess = Substitute.For<IFileAccess>();
			const string fileName = "path";
			fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

			var sut = new ChangeLogSerializer(fileAccess);

			// Act
			var actual = await sut.Deserialize(fileName);

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Collection(version!.Fixed,
				x => Assert.Equal("Change1", x.Text),
				x => Assert.Equal("Multiline" + Environment.NewLine + "change", x.Text),
				x => Assert.Equal("Change3", x.Text)
			);
		}

		[Fact]
		public async Task DeserializeShouldReadMultipleSections()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [Unreleased]");
			sb.AppendLine("### Added");
			sb.AppendLine("- Change1");
			sb.AppendLine("- Multiline");
			sb.AppendLine("  change");
			sb.AppendLine("- Change3");
			sb.AppendLine();
			sb.AppendLine("### Removed");
			sb.AppendLine("- Change4");

			var fileAccess = Substitute.For<IFileAccess>();
			const string fileName = "path";
			fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

			var sut = new ChangeLogSerializer(fileAccess);

			// Act
			var actual = await sut.Deserialize(fileName);

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Collection(version!.Added,
				x => Assert.Equal("Change1", x.Text),
				x => Assert.Equal("Multiline" + Environment.NewLine + "change", x.Text),
				x => Assert.Equal("Change3", x.Text)
			);
			Assert.Collection(version.Removed,
				x => Assert.Equal("Change4", x.Text)
			);
		}

		[Fact]
		public async Task DeserializeShouldReadRemoved()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [Unreleased]");
			sb.AppendLine("### Removed");
			sb.AppendLine("- Change1");
			sb.AppendLine("- Multiline");
			sb.AppendLine("  change");
			sb.AppendLine("- Change3");

			var fileAccess = Substitute.For<IFileAccess>();
			const string fileName = "path";
			fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

			var sut = new ChangeLogSerializer(fileAccess);

			// Act
			var actual = await sut.Deserialize(fileName);

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Collection(version!.Removed,
				x => Assert.Equal("Change1", x.Text),
				x => Assert.Equal("Multiline" + Environment.NewLine + "change", x.Text),
				x => Assert.Equal("Change3", x.Text)
			);
		}

		[Fact]
		public async Task DeserializeShouldReadSecurity()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [Unreleased]");
			sb.AppendLine("### Security");
			sb.AppendLine("- Change1");
			sb.AppendLine("- Multiline");
			sb.AppendLine("  change");
			sb.AppendLine("- Change3");

			var fileAccess = Substitute.For<IFileAccess>();
			const string fileName = "path";
			fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

			var sut = new ChangeLogSerializer(fileAccess);

			// Act
			var actual = await sut.Deserialize(fileName);

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Collection(version!.Security,
				x => Assert.Equal("Change1", x.Text),
				x => Assert.Equal("Multiline" + Environment.NewLine + "change", x.Text),
				x => Assert.Equal("Change3", x.Text)
			);
		}

		[Fact]
		public async Task DeserializeShouldReadVersionDate()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [1.2] - 2020-05-12");

			var fileAccess = Substitute.For<IFileAccess>();
			const string fileName = "path";
			fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

			var sut = new ChangeLogSerializer(fileAccess);

			// Act
			var actual = await sut.Deserialize(fileName);

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Equal(new DateTime(2020, 5, 12), version!.ReleaseDate);
			Assert.False(version.Yanked);
		}

		[Fact]
		public async Task DeserializeShouldReadYankedVersion()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [1.2] - 2020-05-12 [YANKED]");

			var fileAccess = Substitute.For<IFileAccess>();
			const string fileName = "path";
			fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

			var sut = new ChangeLogSerializer(fileAccess);

			// Act
			var actual = await sut.Deserialize(fileName);

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.True(version!.Yanked);
		}

		[Fact]
		public void SerializeShouldContainEntireChanges()
		{
			// Arrange
			var changeLog = new ChangeLog
			{
				Versions = new List<Version>
				{
					new Version
					{
						ReleaseDate = new DateTime(2019, 4, 26),
						Name = "1.2.3",
						Added = new List<Change>
						{
							new Change {Text = "AddA"},
							new Change {Text = "AddB"},
							new Change {Text = "AddC"}
						},
						Deprecated = new List<Change>
						{
							new Change {Text = "DeprecatedA"},
							new Change {Text = "DeprecatedB"},
							new Change {Text = "DeprecatedC"}
						},
						Fixed = new List<Change>
						{
							new Change {Text = "FixedA"},
							new Change {Text = "FixedB"},
							new Change {Text = "FixedC"}
						},
						Removed = new List<Change>
						{
							new Change {Text = "RemovedA"},
							new Change {Text = "RemovedB"},
							new Change {Text = "RemovedC"}
						},
						Security = new List<Change>
						{
							new Change {Text = "SecurityA"},
							new Change {Text = "SecurityB"},
							new Change {Text = "SecurityC"}
						},
						Changed = new List<Change>
						{
							new Change {Text = "ChangedA"},
							new Change {Text = "ChangedB"},
							new Change {Text = "ChangedC"}
						}
					}
				}
			};

			var sut = new ChangeLogSerializer(Substitute.For<IFileAccess>());

			// Act
			var actual = sut.Serialize(changeLog);

			// Assert
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
		public void SerializeShouldContainReleasedDateAndVersionName()
		{
			// Arrange
			var changeLog = new ChangeLog
			{
				Versions = new List<Version>
				{
					new Version
					{
						ReleaseDate = new DateTime(2019, 4, 26),
						Name = "1.2.3"
					}
				}
			};

			var sut = new ChangeLogSerializer(Substitute.For<IFileAccess>());

			// Act
			var actual = sut.Serialize(changeLog);

			// Assert
			Assert.Contains("[1.2.3]", actual);
			Assert.Contains("2019-04-26", actual);
		}

		[Fact]
		public void SerializeShouldContainUnreleasedVersion()
		{
			// Arrange
			var changeLog = new ChangeLog
			{
				Versions = new List<Version>
				{
					new Version
					{
						ReleaseDate = null
					}
				}
			};

			var sut = new ChangeLogSerializer(Substitute.For<IFileAccess>());

			// Act
			var actual = sut.Serialize(changeLog);

			// Assert
			Assert.Contains("[Unreleased]", actual);
		}

		[Fact]
		public void SerializeShouldCreateChangeLogWithCorrectName()
		{
			// Arrange
			var changeLog = new ChangeLog
			{
				UsesSemVer = true
			};

			var sut = new ChangeLogSerializer(Substitute.For<IFileAccess>());

			// Act
			var actual = sut.Serialize(changeLog);

			// Assert
			Assert.NotEmpty(actual);
			Assert.Contains("keepachangelog.com", actual);
			Assert.Contains("semver.org", actual);
		}

		[Fact]
		public void SerializeShouldIndicateYankedReleases()
		{
			// Arrange
			var changeLog = new ChangeLog
			{
				Versions = new List<Version>
				{
					new Version
					{
						ReleaseDate = new DateTime(2019, 4, 26),
						Name = "1.2.3",
						Yanked = true
					}
				}
			};

			var sut = new ChangeLogSerializer(Substitute.For<IFileAccess>());

			// Act
			var actual = sut.Serialize(changeLog);

			// Assert
			Assert.Contains("[YANKED]", actual);
		}

		[Fact]
		public void SerializeShouldNotWriteSemVerInfoWhenDisabled()
		{
			// Arrange
			var changeLog = new ChangeLog
			{
				UsesSemVer = false
			};

			var sut = new ChangeLogSerializer(Substitute.For<IFileAccess>());

			// Act
			var actual = sut.Serialize(changeLog);

			// Assert
			Assert.NotEmpty(actual);
			Assert.DoesNotContain("semver.org", actual);
		}
	}
}