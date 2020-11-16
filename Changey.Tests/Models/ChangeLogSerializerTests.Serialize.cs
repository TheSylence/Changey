using System;
using System.Collections.Generic;
using Changey.Models;
using Xunit;
using Version = Changey.Models.Version;

namespace Changey.Tests.Models
{
	public partial class ChangeLogSerializerTests
	{
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

			var sut = new ChangeLogSerializer();

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

			var sut = new ChangeLogSerializer();

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

			var sut = new ChangeLogSerializer();

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

			var sut = new ChangeLogSerializer();

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

			var sut = new ChangeLogSerializer();

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

			var sut = new ChangeLogSerializer();

			// Act
			var actual = sut.Serialize(changeLog);

			// Assert
			Assert.NotEmpty(actual);
			Assert.DoesNotContain("semver.org", actual);
		}
	}
}