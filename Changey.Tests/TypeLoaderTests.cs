using System.Linq;
using Changey.Commands;
using Changey.Options;
using Xunit;

namespace Changey.Tests
{
	public class TypeLoaderTests
	{
		private class MockOption : BaseOption
		{
			public MockOption()
				: base(string.Empty, false, false)
			{
			}
		}

		public static TheoryData<object> SectionOptions => new()
		{
			new AddOption("test", string.Empty, false, false),
			new ChangeOption("test", string.Empty, false, false),
			new RemoveOption("test", string.Empty, false, false),
			new FixOption("test", string.Empty, false, false),
			new SecurityOption("test", string.Empty, false, false),
			new DeprecatedOption("test", string.Empty, false, false)
		};

		[Fact]
		public void FindCommandShouldFindForInitOptions()
		{
			// Arrange
			var option = new InitOption(true, true, string.Empty, false, false);
			var sut = new TypeLoader();

			// Act
			var actual = sut.FindCommand(option);

			// Assert
			Assert.IsType<InitCommand>(actual);
		}

		[Theory]
		[MemberData(nameof(SectionOptions))]
		public void FindCommandShouldFindForSectionOptions(object arg)
		{
			// Arrange
			var option = (SectionOption) arg;
			var sut = new TypeLoader();

			// Act
			var actual = sut.FindCommand(option);

			// Assert
			Assert.IsType<SectionCommand>(actual);
		}

		[Fact]
		public void FindCommandShouldFindForYankOption()
		{
			// Arrange
			var option = new YankOption(string.Empty, false, false);
			var sut = new TypeLoader();

			// Act
			var actual = sut.FindCommand(option);

			// Assert
			Assert.IsType<YankCommand>(actual);
		}

		[Fact]
		public void FindCommandShouldForReleaseOption()
		{
			// Arrange
			var option = new ReleaseOption(null, string.Empty, string.Empty, false, false);
			var sut = new TypeLoader();

			// Act
			var actual = sut.FindCommand(option);

			// Assert
			Assert.IsType<ReleaseCommand>(actual);
		}

		[Fact]
		public void FindCommandShouldThrowForUnknownCommand()
		{
			// Arrange
			var sut = new TypeLoader();

			// Act
			var ex = Record.Exception(() => sut.FindCommand(new MockOption()));

			// Assert
			Assert.NotNull(ex);
		}

		[Fact]
		public void LoadCommandTypesShouldReturnNonEmptyArray()
		{
			// Arrange
			var sut = new TypeLoader();

			// Act
			var actual = sut.LoadOptionTypes().ToArray();

			// Assert
			Assert.NotEmpty(actual);
		}
	}
}