using System.Threading.Tasks;
using Changey.Commands;
using Changey.Models;
using Changey.Options;
using Changey.Services;
using NSubstitute;
using Xunit;

namespace Changey.Tests.Commands
{
	public class RemoveCommandTests
	{
		[Fact]
		public async Task ExecuteShouldCallSectionAdder()
		{
			// Arrange
			var sectionAdder = Substitute.For<ISectionAdder>();
			const string fileName = "path";
			const string message = "the-message";
			var option = new RemoveOption(message, fileName, false, false);
			var sut = new SectionCommand(option, sectionAdder);

			// Act
			await sut.Execute();

			// Assert
			await sectionAdder.Received(1).AddToSection(fileName, Section.Removed, message);
		}
	}
}