using Changey.Services;
using Xunit;

namespace Changey.Tests.Services;

public class CompareTemplateRepositoryTests
{
    [Theory]
    [InlineData("http://unsecure.com")]
    [InlineData("https://example.com")]
    [InlineData("example.com")]
    [InlineData("domainWithoutTld.")]
    [InlineData("example.com/host/github.com")]
    [InlineData("github.com.phishing.site/login")]
    public void TemplateShouldBeNullWhenUnknownHost(string url)
    {
        // Arrange
        var sut = new CompareTemplateRepository();

        // Act
        var actual = sut.TemplateFor(url);

        // Assert
        Assert.Null(actual);
    }

    [Theory]
    [InlineData("https://github.com/myorg/myrepo", "%URL%/compare/%OLD_VERSION%...%NEW_VERSION%")]
    [InlineData("https://gitlab.com/myorg/myrepo", "%URL%/compare/%OLD_VERSION%...%NEW_VERSION%")]
    [InlineData("https://github.com/TheSylence/changey", "%URL%/compare/%OLD_VERSION%...%NEW_VERSION%")]
    public void TemplateShouldContainCorrectCompare(string url, string expected)
    {
        // Arrange
        var sut = new CompareTemplateRepository();

        // Act
        var actual = sut.TemplateFor(url);

        // Assert
        Assert.NotNull(actual);
        Assert.Contains(expected, actual!.Compare);
    }

    [Theory]
    [InlineData("https://github.com/myorg/myrepo", "%URL%/releases/tag/%VERSION%")]
    [InlineData("https://gitlab.com/myorg/myrepo", "%URL%/releases/tag/%VERSION%")]
    [InlineData("https://github.com/TheSylence/changey", "%URL%/releases/tag/%VERSION%")]
    public void TemplateShouldContainCorrectRelease(string url, string expected)
    {
        // Arrange
        var sut = new CompareTemplateRepository();

        // Act
        var actual = sut.TemplateFor(url);

        // Assert
        Assert.NotNull(actual);
        Assert.Contains(expected, actual!.Release);
    }
}