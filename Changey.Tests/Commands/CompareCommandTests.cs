using System.Threading.Tasks;
using Changey.Commands;
using Changey.Options;
using Changey.Services;
using NSubstitute;
using Xunit;

namespace Changey.Tests.Commands;

public class CompareCommandTests
{
    [Fact]
    public async Task ExecuteShouldCallCompareGenerator()
    {
        // Arrange
        const string path = "path";
        const string baseUrl = "base";
        const string compareTemplate = "compare";
        const string releaseTemplate = "release";

        var generator = Substitute.For<ICompareGenerator>();
        var option = new CompareOption(baseUrl, compareTemplate, releaseTemplate, path, false, false);
        var sut = new CompareCommand(option, generator);

        // Act
        await sut.Execute();

        // Assert
        await generator.Received(1).Generate(path, baseUrl, compareTemplate, releaseTemplate);
    }
}