using Changey.Commands;
using Xunit;

namespace Changey.Tests.Commands
{
	public class CommandLoaderTests
	{
		[Fact]
		public void LoadCommandTypesShouldReturnNonEmptyArray()
		{
			// Arrange
			var sut = new CommandLoader();

			// Act
			var actual = sut.LoadCommandTypes();

			// Assert
			Assert.NotEmpty(actual);
		}
	}
}