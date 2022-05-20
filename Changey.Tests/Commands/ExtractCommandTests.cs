using System.Threading.Tasks;
using Changey.Commands;
using Changey.Options;
using Changey.Services;
using NSubstitute;
using Xunit;

namespace Changey.Tests.Commands;

public class ExtractCommandTests
{
	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public async Task ExecuteShouldCallSerializer(bool header)
	{
		// Arrange
		var extractor = Substitute.For<IExtractor>();
		var option = new ExtractOption(header, "target", "1.2", "source", false, false);

		var sut = new ExtractCommand(option, extractor);

		// Act
		await sut.Execute();

		// Assert
		await extractor.Received(1).Extract("source", "1.2", "target", header);
	}
}