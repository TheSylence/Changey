using System.Threading.Tasks;
using Changey.Commands;
using Changey.Models;
using Changey.Services;
using NSubstitute;
using Xunit;

namespace Changey.Tests.Commands
{
	public class AddCommandTests
	{
		[Fact]
		public async Task ExecuteShouldCallSectionAdder()
		{
			// Arrange
			var sectionAdder = Substitute.For<ISectionAdder>();
			const string fileName = "path";
			const string message = "the-message";
			var sut = new AddCommand(message, false, false, fileName, sectionAdder);

			// Act
			await sut.Execute();

			// Assert
			await sectionAdder.Received(1).AddToSection(fileName, Section.Added, message);
		}
	}
}