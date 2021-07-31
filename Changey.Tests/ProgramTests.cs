using System;
using System.IO;
using System.Threading.Tasks;
using Changey.Commands;
using Changey.Options;
using NSubstitute;
using Xunit;

namespace Changey.Tests
{
	public class ProgramTests
	{
		private static string[] SplitArgs(string args) => args.Split(' ');

		[Fact]
		public async Task AddShouldTreatUnnamedOptionAsMessage()
		{
			// Arrange
			const string path = nameof(AddShouldTreatUnnamedOptionAsMessage) + ".md";
			await using var writer = new StringWriter();
			var sut = new Program(null, writer);

			var args = SplitArgs($"init -p {path} -o");
			await sut.Run(args);

			args = SplitArgs($"add test123 -p {path}");

			// Act
			await sut.Run(args);

			// Assert
			var output = writer.ToString();
			Assert.DoesNotContain("ERROR", output);

			var content = await File.ReadAllTextAsync(path);
			Assert.Contains("test123", content);
		}

		[Fact]
		public async Task EmptyArgumentsShouldProduceHelpScreen()
		{
			// Arrange
			await using var writer = new StringWriter();
			var sut = new Program(null, writer);

			// Act
			await sut.Run(Array.Empty<string>());

			// Assert
			var output = writer.ToString();
			Assert.Contains("version", output);
			Assert.Contains("help", output);
		}

		[Theory]
		[InlineData("init")]
		[InlineData("release")]
		[InlineData("add")]
		[InlineData("security")]
		[InlineData("change")]
		[InlineData("remove")]
		[InlineData("fix")]
		[InlineData("deprecate")]
		public async Task HelpScreenShouldContainExamples(string command)
		{
			// Arrange
			await using var writer = new StringWriter();
			var sut = new Program(null, writer);

			// Act
			await sut.Run(new[] {"help", command});

			// Assert
			var output = writer.ToString();
			Assert.Contains("USAGE", output);
		}

		[Fact]
		public async Task HelpShouldDisplayHelp()
		{
			// Arrange
			await using var writer = new StringWriter();
			var sut = new Program(null, writer);

			// Act
			await sut.Run(new[] {"--help"});

			// Assert
			var output = writer.ToString();
			Assert.Contains("help", output);

			var expectedVerbs = new[]
			{
				"init", "release", "yank", "add", "change", "fix",
				"security", "remove", "deprecate", "version"
			};

			foreach (var expectedVerb in expectedVerbs)
			{
				Assert.Contains(expectedVerb, output);
			}
		}

		[Theory]
		[InlineData("init")]
		[InlineData("release")]
		[InlineData("yank")]
		[InlineData("add")]
		[InlineData("security")]
		[InlineData("change")]
		[InlineData("remove")]
		[InlineData("fix")]
		[InlineData("deprecate")]
		public async Task HelpShouldDisplayHelpWhenVerbIsSpecified(string verb)
		{
			// Arrange
			await using var writer = new StringWriter();
			var sut = new Program(null, writer);

			var args = new[] {"help", verb};

			// Act
			await sut.Run(args);

			// Assert
			var output = writer.ToString();
			Assert.DoesNotContain($"  {verb} ", output);
		}

		[Theory]
		[InlineData("release", "version to release")]
		[InlineData("add", "message that should be added")]
		[InlineData("security", "message that should be added")]
		[InlineData("deprecate", "message that should be added")]
		[InlineData("fix", "message that should be added")]
		[InlineData("change", "message that should be added")]
		[InlineData("remove", "message that should be added")]
		public async Task MissingArgsShouldProduceError(string verb, string missingArg)
		{
			// Arrange
			await using var writer = new StringWriter();
			var sut = new Program(null, writer);

			var args = SplitArgs(verb);

			// Act
			await sut.Run(args);

			// Assert
			var output = writer.ToString();
			Assert.Contains("ERROR", output);
			Assert.Contains("Required", output);
			Assert.Contains(missingArg, output);
		}

		[Theory]
		[InlineData("init", typeof(InitOption))]
		[InlineData("release test", typeof(ReleaseOption))]
		[InlineData("yank", typeof(YankOption))]
		[InlineData("add test", typeof(AddOption))]
		[InlineData("security test", typeof(SecurityOption))]
		[InlineData("deprecate test", typeof(DeprecatedOption))]
		[InlineData("fix test", typeof(FixOption))]
		[InlineData("change test", typeof(ChangeOption))]
		[InlineData("remove test", typeof(RemoveOption))]
		public async Task VerbShouldUseCorrectCommand(string verb, Type optionType)
		{
			// Arrange
			var command = Substitute.For<ICommand>();
			command.Execute().Returns(Task.CompletedTask);

			var wrappedTypeLoader = new TypeLoader();
			var typeLoader = Substitute.For<ITypeLoader>();
			typeLoader.LoadOptionTypes().Returns(wrappedTypeLoader.LoadOptionTypes());
			typeLoader.FindCommand(Arg.Is<BaseOption>(o => o.GetType().IsAssignableFrom(optionType))).Returns(command);
			var sut = new Program(typeLoader, null);

			var args = SplitArgs(verb);

			// Act
			await sut.Run(args);

			// Assert
			await command.Received(1).Execute();
		}
	}
}