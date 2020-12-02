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
		private string[] SplitArgs(string args) => args.Split(' ');

		[Fact]
		public async Task EmptyArgumentsShouldProduceHelpScreen()
		{
			// Arrange
			await using var writer = new StringWriter();
			var sut = new Program(null, writer);

			// Act
			await sut.Run(new string[0]);

			// Assert
			var output = writer.ToString();
			Assert.Contains("version", output);
			Assert.Contains("help", output);
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
			Assert.DoesNotContain($"  {verb}", output);
		}

		[Theory]
		[InlineData("release", "-n")]
		[InlineData("add", "-m")]
		[InlineData("security", "-m")]
		[InlineData("deprecate", "-m")]
		[InlineData("fix", "-m")]
		[InlineData("change", "-m")]
		[InlineData("remove", "-m")]
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
		[InlineData("release -n test", typeof(ReleaseOption))]
		[InlineData("yank", typeof(YankOption))]
		[InlineData("add -m test", typeof(AddOption))]
		[InlineData("security -m test", typeof(SecurityOption))]
		[InlineData("deprecate -m test", typeof(DeprecatedOption))]
		[InlineData("fix -m test", typeof(FixOption))]
		[InlineData("change -m test", typeof(ChangeOption))]
		[InlineData("remove -m test", typeof(RemoveOption))]
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