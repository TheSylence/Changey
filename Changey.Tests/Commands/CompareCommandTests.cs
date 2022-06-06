using System;
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

	[Fact]
	public async Task ExecuteShouldLogWhenCreatorThrows()
	{
		// Arrange
		var compareGenerator = Substitute.For<ICompareGenerator>();
		compareGenerator.Generate(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
			.Returns(Task.FromException<bool>(new Exception("test-exception")));

		var option = new CompareOption("", "", "", "", false, false);

		var logger = Substitute.For<ILogger>();
		option.InjectLogger(logger);

		var sut = new CompareCommand(option, compareGenerator);

		// Act
		await sut.Execute();

		// Assert
		logger.Received(1).Error(Arg.Any<string>(), Arg.Is<Exception>(e => e.Message.Contains("test-exception")));
	}
}