using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Changey.Tests
{
	public class IntegrationTests
	{
		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async Task ExtractionTest(bool header)
		{
			// Arrange
			File.Delete("output.md");

			await using var writer = new StringWriter();
			var sut = new Program(null, writer);

			var test = new IntegrationTestRunner(sut);
			test.AddPass("init", new[] { "changelog" }, Array.Empty<string>());
			test.AddPass("add \"Change message\"", new[] { "Change message" }, Array.Empty<string>());
			test.AddPass("release 0.9.1", new[] { "0.9.1" }, Array.Empty<string>());

			if (header)
				test.AddPass("extract 0.9.1 -t output.md -h", Array.Empty<string>(), Array.Empty<string>());
			else
				test.AddPass("extract 0.9.1 -t output.md", Array.Empty<string>(), Array.Empty<string>());

			// Act
			var actual = await test.Run(nameof(ExtractionTest) + "_changelog.md");

			// Assert
			Assert.Empty(actual);

			var extractedContent = await File.ReadAllLinesAsync("output.md");
			Assert.Contains(extractedContent, l => l.Contains("Change message"));
			if (header)
				Assert.Contains(extractedContent, l => l.Contains("0.9.1"));
			else
				Assert.DoesNotContain(extractedContent, l => l.Contains("0.9.1"));
		}

		[Fact]
		public async Task IntegrationTestRun()
		{
			// Arrange
			await using var writer = new StringWriter();
			var sut = new Program(null, writer);

			var test = new IntegrationTestRunner(sut);
			test.AddPass("init", new[] { "changelog" }, Array.Empty<string>());
			test.AddPass("add \"First message\"", new[] { "Added", "First message", "[Unreleased]" },
				Array.Empty<string>());
			test.AddPass("release 1.0", new[] { "[1.0] - " }, new[] { "[Unreleased]" });
			test.AddPass("change \"Second message\"", new[] { "Changed", "Second message" }, Array.Empty<string>());
			test.AddPass("release 1.1", new[] { "[1.1] - " }, new[] { "[Unreleased]" });
			test.AddPass("yank", new[] { "[YANKED]" }, Array.Empty<string>());

			// Act
			var actual = await test.Run(nameof(IntegrationTestRun) + "_changelog.md");

			// Assert
			Assert.Empty(actual);
		}
	}
}