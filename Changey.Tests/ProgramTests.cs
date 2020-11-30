using System;
using System.Threading.Tasks;
using Changey.Commands;
using Changey.Options;
using NSubstitute;
using Xunit;

namespace Changey.Tests
{
	public class ProgramTests
	{
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
			var sut = new Program(typeLoader);

			var args = SplitArgs(verb);

			// Act
			await sut.Run(args);

			// Assert
			await command.Received(1).Execute();
		}

		private string[] SplitArgs(string args) => args.Split(' ');
	}
}