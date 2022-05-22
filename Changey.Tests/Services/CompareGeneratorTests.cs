using System;
using System.Linq;
using System.Threading.Tasks;
using Changey.Models;
using Changey.Services;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;
using Version = Changey.Models.Version;

namespace Changey.Tests.Services;

public class CompareGeneratorTests
{
    private const string BaseUrl = "github.com/TheSylence/Changey";

    private static CompareGenerator GenerateSut(ChangeLog existingChangeLog)
    {
        var logger = Substitute.For<ILogger>();
        var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
        changeLogSerializer.Deserialize(Arg.Any<string>()).Returns(existingChangeLog);
        var sut = new CompareGenerator(logger, changeLogSerializer);
        return sut;
    }

    [Fact]
    public async Task ExecuteShouldClearUrlForUnreleasedVersionWhenNoOtherVersionExists()
    {
        // Arrange
        var existingChangeLog = new ChangeLog();
        existingChangeLog.Versions.Add(new Version
        {
            Name = "Unreleased"
        });

        var sut = GenerateSut(existingChangeLog);

        // Act
        var result = await sut.Generate("", BaseUrl, "", "");

        // Assert
        Assert.True(result);
        Assert.Empty(existingChangeLog.Versions[0].CompareUrl);
    }

    [Fact]
    public async Task ExecuteShouldFailWhenNoCompareTemplateGiven()
    {
        // Arrange
        var existingChangeLog = new ChangeLog();
        var sut = GenerateSut(existingChangeLog);

        // Act
        var result = await sut.Generate("", "base", "", "release");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExecuteShouldFailWhenNoReleaseTemplateGiven()
    {
        // Arrange
        var existingChangeLog = new ChangeLog();
        var sut = GenerateSut(existingChangeLog);

        // Act
        var result = await sut.Generate("", "base", "compare", "");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExecuteShouldNotCrashWhenChangeLogIsEmpty()
    {
        // Arrange
        var existingChangeLog = new ChangeLog();

        var sut = GenerateSut(existingChangeLog);

        // Act
        var ex = await Record.ExceptionAsync(async () => await sut.Generate("", BaseUrl, "", ""));

        // Assert
        Assert.Null(ex);
    }

    [Fact]
    public async Task ExecuteShouldSetUrlForFirstVersion()
    {
        // Arrange
        var existingChangeLog = new ChangeLog();
        existingChangeLog.Versions.Add(new Version
        {
            Name = "0.1",
            ReleaseDate = new DateTime(2005, 3, 8)
        });

        var sut = GenerateSut(existingChangeLog);

        // Act
        var result = await sut.Generate("", BaseUrl, "", "");

        // Assert
        Assert.True(result);
        const string expectedUrl = BaseUrl + "/releases/tag/0.1";
        Assert.Equal(expectedUrl, existingChangeLog.Versions[0].CompareUrl);
    }

    [Fact]
    public async Task ExecuteShouldSetUrlForUnreleasedVersion()
    {
        // Arrange
        var existingChangeLog = new ChangeLog();
        existingChangeLog.Versions.Add(new Version
        {
            Name = "Unreleased"
        });
        existingChangeLog.Versions.Add(new Version
        {
            Name = "0.1",
            ReleaseDate = new DateTime(2005, 3, 8)
        });

        var sut = GenerateSut(existingChangeLog);

        // Act
        var result = await sut.Generate("", BaseUrl, "", "");

        // Assert
        Assert.True(result);
        Assert.Collection(existingChangeLog.Versions.Select(v => v.CompareUrl),
            url => Assert.Equal(BaseUrl + "/compare/0.1...HEAD", url),
            url => Assert.Equal(BaseUrl + "/releases/tag/0.1", url)
        );
    }

    [Fact]
    public async Task ExecuteShouldSetUrlsForVersions()
    {
        // Arrange
        var existingChangeLog = new ChangeLog();
        existingChangeLog.Versions.Add(new Version
        {
            Name = "Unreleased"
        });
        existingChangeLog.Versions.Add(new Version
        {
            Name = "0.2",
            ReleaseDate = new DateTime(2005, 4, 8)
        });
        existingChangeLog.Versions.Add(new Version
        {
            Name = "0.1",
            ReleaseDate = new DateTime(2005, 3, 8)
        });

        var sut = GenerateSut(existingChangeLog);

        // Act
        var result = await sut.Generate("", BaseUrl, "", "");

        // Assert
        Assert.True(result);
        Assert.Collection(existingChangeLog.Versions.Select(v => v.CompareUrl),
            url => Assert.Equal(BaseUrl + "/compare/0.2...HEAD", url),
            url => Assert.Equal(BaseUrl + "/compare/0.1...0.2", url),
            url => Assert.Equal(BaseUrl + "/releases/tag/0.1", url)
        );
    }

    [Fact]
    public async Task ExecuteShouldSetUrlTemplatesOfChangeLog()
    {
        // Arrange
        var existingChangeLog = new ChangeLog();
        var sut = GenerateSut(existingChangeLog);

        // Act
        var result = await sut.Generate("", BaseUrl, "", "");

        // Assert
        Assert.True(result);
        Assert.NotEmpty(existingChangeLog.UrlTemplates.Release);
        Assert.NotEmpty(existingChangeLog.UrlTemplates.Compare);
    }

    [Fact]
    public async Task ExecuteShouldUseGivenTemplates()
    {
        // Arrange
        var existingChangeLog = new ChangeLog();
        var sut = GenerateSut(existingChangeLog);

        const string baseUrl = "http://base";

        // Act
        var result = await sut.Generate("", baseUrl, "/compare", "/release");

        // Assert
        Assert.True(result);
        Assert.Equal("/compare", existingChangeLog.UrlTemplates.Compare);
        Assert.Equal("/release", existingChangeLog.UrlTemplates.Release);
    }

    [Fact]
    public async Task GenerateShouldFailWhenSerializationFails()
    {
        // Arrange
        var existingChangeLog = new ChangeLog();
        existingChangeLog.Versions.Add(new Version());
        var logger = Substitute.For<ILogger>();
        var changeLogSerializer = Substitute.For<IChangeLogSerializer>();
        changeLogSerializer.Deserialize(Arg.Any<string>()).Returns(existingChangeLog);
        changeLogSerializer.Serialize(Arg.Any<ChangeLog>(), Arg.Any<string>()).Throws(new Exception("test exception"));

        var sut = new CompareGenerator(logger, changeLogSerializer);

        // Act
        var result = await sut.Generate("", BaseUrl, "", "");

        // Assert
        Assert.False(result);
    }
}