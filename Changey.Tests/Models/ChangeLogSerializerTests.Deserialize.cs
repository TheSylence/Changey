using System;
using System.Linq;
using System.Text;
using Changey.Models;
using Xunit;

namespace Changey.Tests.Models
{
	public partial class ChangeLogSerializerTests
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
		public void DeserializeShouldDetermineThatSemVerIsNotUsed()
		{
			// Arrange
			var sut = new ChangeLogSerializer();

			var sb = new StringBuilder();
			sb.AppendLine("# Changelog");
			sb.AppendLine("All notable changes to this project will be documented in this file.");
			sb.AppendLine();
			sb.AppendLine("The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).");
			sb.AppendLine();

			// Act
			var actual = sut.DeserializeFromContent(sb.ToString());

			// Assert
			Assert.NotNull(actual);
			Assert.False(actual.UsesSemVer);
		}

		[Fact]
		public void DeserializeShouldDetermineThatSemVerIsUsed()
		{
			// Arrange
			var sut = new ChangeLogSerializer();

			var sb = new StringBuilder();
			sb.AppendLine("# Changelog");
			sb.AppendLine("All notable changes to this project will be documented in this file.");
			sb.AppendLine();
			sb.AppendLine("The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),");
			sb.AppendLine("and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).");
			sb.AppendLine();

			// Act
			var actual = sut.DeserializeFromContent(sb.ToString());

			// Assert
			Assert.NotNull(actual);
			Assert.True(actual.UsesSemVer);
		}

		[Fact]
		public void DeserializeShouldHandleEmptySection()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [Unreleased]");
			sb.Append("### Security");

			var sut = new ChangeLogSerializer();

			// Act
			var actual = sut.DeserializeFromContent(sb.ToString());

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Empty(version.Security);
		}

		[Fact]
		public void DeserializeShouldHandleUnreleasedVersion()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [Unreleased]");

			var sut = new ChangeLogSerializer();

			// Act
			var actual = sut.DeserializeFromContent(sb.ToString());

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Null(version.ReleaseDate);
		}

		[Fact]
		public void DeserializeShouldNotParseChangelogWithoutHeader()
		{
			// Arrange
			var sut = new ChangeLogSerializer();
			var sb = new StringBuilder();

			// Act
			var actual = sut.DeserializeFromContent(sb.ToString());

			// Assert
			Assert.NotNull(actual);
			Assert.False(actual.UsesSemVer);
			Assert.Empty(actual.Versions);
		}

		[Fact]
		public void DeserializeShouldNotParseChangelogWithoutSpecUrl()
		{
			// Arrange
			var sut = new ChangeLogSerializer();

			var sb = new StringBuilder();
			sb.AppendLine("# Changelog");
			sb.AppendLine("All notable changes to this project will be documented in this file.");
			sb.AppendLine();

			// Act
			var actual = sut.DeserializeFromContent(sb.ToString());

			// Assert
			Assert.NotNull(actual);
			Assert.False(actual.UsesSemVer);
			Assert.Empty(actual.Versions);
		}

		[Fact]
		public void DeserializeShouldReadAdded()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [Unreleased]");
			sb.AppendLine("### Added");
			sb.AppendLine("- Change1");
			sb.AppendLine("- Multiline");
			sb.AppendLine("  change");
			sb.AppendLine("- Change3");

			var sut = new ChangeLogSerializer();

			// Act
			var actual = sut.DeserializeFromContent(sb.ToString());

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Collection(version.Added,
				x => Assert.Equal("Change1", x.Text),
				x => Assert.Equal("Multiline" + Environment.NewLine + "change", x.Text),
				x => Assert.Equal("Change3", x.Text)
			);
		}

		[Fact]
		public void DeserializeShouldReadChanged()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [Unreleased]");
			sb.AppendLine("### Changed");
			sb.AppendLine("- Change1");
			sb.AppendLine("- Multiline");
			sb.AppendLine("  change");
			sb.AppendLine("- Change3");

			var sut = new ChangeLogSerializer();

			// Act
			var actual = sut.DeserializeFromContent(sb.ToString());

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Collection(version.Changed,
				x => Assert.Equal("Change1", x.Text),
				x => Assert.Equal("Multiline" + Environment.NewLine + "change", x.Text),
				x => Assert.Equal("Change3", x.Text)
			);
		}

		[Fact]
		public void DeserializeShouldReadDeprecated()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [Unreleased]");
			sb.AppendLine("### Deprecated");
			sb.AppendLine("- Change1");
			sb.AppendLine("- Multiline");
			sb.AppendLine("  change");
			sb.AppendLine("- Change3");

			var sut = new ChangeLogSerializer();

			// Act
			var actual = sut.DeserializeFromContent(sb.ToString());

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Collection(version.Deprecated,
				x => Assert.Equal("Change1", x.Text),
				x => Assert.Equal("Multiline" + Environment.NewLine + "change", x.Text),
				x => Assert.Equal("Change3", x.Text)
			);
		}

		[Fact]
		public void DeserializeShouldReadFixed()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [Unreleased]");
			sb.AppendLine("### Fixed");
			sb.AppendLine("- Change1");
			sb.AppendLine("- Multiline");
			sb.AppendLine("  change");
			sb.AppendLine("- Change3");

			var sut = new ChangeLogSerializer();

			// Act
			var actual = sut.DeserializeFromContent(sb.ToString());

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Collection(version.Fixed,
				x => Assert.Equal("Change1", x.Text),
				x => Assert.Equal("Multiline" + Environment.NewLine + "change", x.Text),
				x => Assert.Equal("Change3", x.Text)
			);
		}

		[Fact]
		public void DeserializeShouldReadMultipleSections()
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

			var sut = new ChangeLogSerializer();

			// Act
			var actual = sut.DeserializeFromContent(sb.ToString());

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Collection(version.Added,
				x => Assert.Equal("Change1", x.Text),
				x => Assert.Equal("Multiline" + Environment.NewLine + "change", x.Text),
				x => Assert.Equal("Change3", x.Text)
			);
			Assert.Collection(version.Removed,
				x => Assert.Equal("Change4", x.Text)
			);
		}

		[Fact]
		public void DeserializeShouldReadRemoved()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [Unreleased]");
			sb.AppendLine("### Removed");
			sb.AppendLine("- Change1");
			sb.AppendLine("- Multiline");
			sb.AppendLine("  change");
			sb.AppendLine("- Change3");

			var sut = new ChangeLogSerializer();

			// Act
			var actual = sut.DeserializeFromContent(sb.ToString());

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Collection(version.Removed,
				x => Assert.Equal("Change1", x.Text),
				x => Assert.Equal("Multiline" + Environment.NewLine + "change", x.Text),
				x => Assert.Equal("Change3", x.Text)
			);
		}

		[Fact]
		public void DeserializeShouldReadSecurity()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [Unreleased]");
			sb.AppendLine("### Security");
			sb.AppendLine("- Change1");
			sb.AppendLine("- Multiline");
			sb.AppendLine("  change");
			sb.AppendLine("- Change3");

			var sut = new ChangeLogSerializer();

			// Act
			var actual = sut.DeserializeFromContent(sb.ToString());

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Collection(version.Security,
				x => Assert.Equal("Change1", x.Text),
				x => Assert.Equal("Multiline" + Environment.NewLine + "change", x.Text),
				x => Assert.Equal("Change3", x.Text)
			);
		}

		[Fact]
		public void DeserializeShouldReadVersionDate()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [1.2] - 2020-05-12");

			var sut = new ChangeLogSerializer();

			// Act
			var actual = sut.DeserializeFromContent(sb.ToString());

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.Equal(new DateTime(2020, 5, 12), version.ReleaseDate);
			Assert.False(version.Yanked);
		}

		[Fact]
		public void DeserializeShouldReadYankedVersion()
		{
			// Arrange
			var sb = GenerateChangeLogHeader();
			sb.AppendLine("## [1.2] - 2020-05-12 [YANKED]");

			var sut = new ChangeLogSerializer();

			// Act
			var actual = sut.DeserializeFromContent(sb.ToString());

			// Assert
			var version = actual.Versions.FirstOrDefault();
			Assert.NotNull(version);
			Assert.True(version.Yanked);
		}
	}
}