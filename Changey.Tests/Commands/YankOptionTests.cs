using System.Threading.Tasks;
using Changey.Commands;
using Changey.Options;
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
			var option = new YankOption("path", false, false);
			var sut = new YankCommand(option, versionYanker);

			// Act
			await sut.Execute();

			// Assert
			await versionYanker.Received(1).Yank("path");
		}
	}
}