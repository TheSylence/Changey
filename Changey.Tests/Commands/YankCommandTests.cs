using System.Threading.Tasks;
using Changey.Commands;
using Changey.Services;
using NSubstitute;
using Xunit;

namespace Changey.Tests.Commands
{
	public class YankCommandTests
	{
		[Fact]
		public async Task ExecuteShouldCallYanker()
		{
			// Arrange
			var versionYanker = Substitute.For<IVersionYanker>();
			var sut = new YankCommand(false, false, "path", versionYanker);

			// Act
			await sut.Execute();

			// Assert
			await versionYanker.Received(1).Yank("path");
		}
	}
}