using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Changey.Services;
using NSubstitute;
using Xunit;

namespace Changey.Tests.Services;

public partial class ChangeLogSerializerTests
{
	
	[Fact]
	public async Task DeserializeShouldDetermineThatSemVerIsNotUsed()
	{
		// Arrange
		var sb = GenerateChangeLogHeader(false);

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
		var sb = GenerateChangeLogHeader();

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
	public async Task DeserializeShouldHandleInvalidVersions()
	{
		// Arrange
		var sb = GenerateChangeLogHeader();
		sb.AppendLine("## 1.a.0");

		var fileAccess = Substitute.For<IFileAccess>();
		const string fileName = "path";
		fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

		var sut = new ChangeLogSerializer(fileAccess);

		// Act
		var actual = await sut.Deserialize(fileName);

		// Assert
		Assert.Empty(actual.Versions);
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
	public async Task DeserializeShouldIncludeCompareUrls()
	{
		// Arrange
		var sb = GenerateChangeLogHeader();
		sb.AppendLine("## [Unreleased]");
		sb.AppendLine("### Fixed");
		sb.AppendLine("- Test1");
		sb.AppendLine("## [1.0]");
		sb.AppendLine("### Fixed");
		sb.AppendLine("- Test2");
		sb.AppendLine();
		sb.AppendLine("[Unreleased]: http://example.com/latest..1.0");
		sb.AppendLine("[1.0]: http://example.com/1.0");

		var fileAccess = Substitute.For<IFileAccess>();
		const string fileName = "path";
		fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

		var sut = new ChangeLogSerializer(fileAccess);

		// Act
		var actual = await sut.Deserialize(fileName);

		// Assert
		Assert.Equal("http://example.com/latest..1.0", actual.Versions[0].CompareUrl);
		Assert.Equal("http://example.com/1.0", actual.Versions[1].CompareUrl);
	}

	[Fact]
	public async Task DeserializeShouldNotIncludeNonExistingCompareUrls()
	{
		// Arrange
		var sb = GenerateChangeLogHeader();
		sb.AppendLine("## [Unreleased]");
		sb.AppendLine("### Fixed");
		sb.AppendLine("- Test1");
		sb.AppendLine("## [1.0]");
		sb.AppendLine("### Fixed");
		sb.AppendLine("- Test2");

		var fileAccess = Substitute.For<IFileAccess>();
		const string fileName = "path";
		fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

		var sut = new ChangeLogSerializer(fileAccess);

		// Act
		var actual = await sut.Deserialize(fileName);

		// Assert
		Assert.Empty(actual.Versions[0].CompareUrl);
		Assert.Empty(actual.Versions[1].CompareUrl);
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
	public async Task DeserializeShouldReadCompareUrlTemplates()
	{
		// Arrange
		var sb = GenerateChangeLogHeader();
		sb.AppendLine("<!-- Release: %ReleaseTemplate% -->");
		sb.AppendLine("<!-- Compare: %CompareTemplate% -->");
		sb.AppendLine("<!-- BaseUrl: Base -->");

		var fileAccess = Substitute.For<IFileAccess>();
		const string fileName = "path";
		fileAccess.ReadFromFile(fileName).Returns(Task.FromResult(sb.ToString()));

		var sut = new ChangeLogSerializer(fileAccess);

		// Act
		var actual = await sut.Deserialize(fileName);

		// Assert
		Assert.Equal("%ReleaseTemplate%", actual.UrlTemplates.Release);
		Assert.Equal("%CompareTemplate%", actual.UrlTemplates.Compare);
		Assert.Equal("Base", actual.UrlTemplates.Base);
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
	public async Task DeserializeShouldReadVersionName()
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
		Assert.Equal("1.2", version!.Name);
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
}