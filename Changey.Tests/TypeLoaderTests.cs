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

		public static TheoryData<object> SectionOptions => new TheoryData<object>
		{
			new AddOption("test", false, false, string.Empty),
			new ChangeOption("test", false, false, string.Empty),
			new RemoveOption("test", false, false, string.Empty),
			new FixOption("test", false, false, string.Empty),
			new SecurityOption("test", false, false, string.Empty),
			new DeprecatedOption("test", false, false, string.Empty)
		};

		[Fact]
		public void FindCommandShouldFindForInitOptions()
		{
			// Arrange
			var option = new InitOption(true, string.Empty, false, false);
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
			var option = new YankOption(false, false, string.Empty);
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