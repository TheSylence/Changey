using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Changey.Tests
{
	public class IntegrationTests
	{
		[Fact]
		public async Task IntegrationTestRun()
		{
			// Arrange
			await using var writer = new StringWriter();
			var sut = new Program(null, writer);

			var test = new IntegrationTestRunner(sut);
			test.AddPass("init -p %changelogpath%", new[] {"changelog"}, Array.Empty<string>());
			test.AddPass("add \"First message\" -p %changelogpath%", new[] {"Added", "First message", "[Unreleased]"}, Array.Empty<string>());
			test.AddPass("release 1.0 -p %changelogpath%", new[] {"[1.0] - "}, new[] {"[Unreleased]"});
			test.AddPass("change \"Second message\" -p %changelogpath%", new[] {"Changed", "Second message"}, Array.Empty<string>());
			test.AddPass("release 1.1 -p %changelogpath%", new[] {"[1.1] - "}, new[] {"[Unreleased]"});
			test.AddPass("yank -p %changelogpath%", new[] {"[YANKED]"}, Array.Empty<string>());

			// Act
			var actual = await test.Run(nameof(IntegrationTestRun) + "_changelog.md");

			// Assert
			Assert.Empty(actual);
		}
	}
}