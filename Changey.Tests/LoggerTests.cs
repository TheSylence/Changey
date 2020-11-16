using System;
using System.Diagnostics;
using System.IO;
using Xunit;

namespace Changey.Tests
{
	public class LoggerTests
	{
		[Fact]
		public void ErrorExceptionShouldNotWriteToOutputWhenSilent()
		{
			// Arrange
			using var output = new StringWriter();
			var sut = new Logger(output, true, false);

			// Act
			sut.Error("test", new Exception("test-exception"));

			// Assert
			Assert.Empty(output.ToString());
		}

		[Fact]
		public void ErrorExceptionShouldWriteToOutputWhenNotSilent()
		{
			// Arrange
			using var output = new StringWriter();
			var sut = new Logger(output, false, false);
			var exception = new Exception("test-exception");
			exception.SetStackTrace(new StackTrace());

			// Act
			sut.Error("test", exception);

			// Assert
			var lines = output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
			Assert.True(lines.Length > 3);
			Assert.Equal("test", lines[0]);
			Assert.Contains("test-exception", lines[1]);
			Assert.Contains("at", lines[2]);
			Assert.Contains("at", lines[3]);
		}

		[Fact]
		public void ErrorShouldNotWriteToOutputWhenSilent()
		{
			// Arrange
			using var output = new StringWriter();
			var sut = new Logger(output, true, false);

			// Act
			sut.Error("test");

			// Assert
			Assert.Empty(output.ToString());
		}

		[Fact]
		public void ErrorShouldWriteToOutputWhenNotSilent()
		{
			// Arrange
			using var output = new StringWriter();
			var sut = new Logger(output, false, false);

			// Act
			sut.Error("test");

			// Assert
			Assert.Equal("test" + Environment.NewLine, output.ToString());
		}

		[Fact]
		public void InfoShouldNotWriteToOutputWhenSilent()
		{
			// Arrange
			using var output = new StringWriter();
			var sut = new Logger(output, true, false);

			// Act
			sut.Info("test");

			// Assert
			Assert.Empty(output.ToString());
		}

		[Fact]
		public void InfoShouldWriteToOutputWhenNotSilent()
		{
			// Arrange
			using var output = new StringWriter();
			var sut = new Logger(output, false, false);

			// Act
			sut.Info("test");

			// Assert
			Assert.Equal("test" + Environment.NewLine, output.ToString());
		}

		[Fact]
		public void VerboseShouldNotWriteToOutputWhenDisabled()
		{
			// Arrange
			using var output = new StringWriter();
			var sut = new Logger(output, false, false);

			// Act
			sut.Verbose("test");

			// Assert
			Assert.Empty(output.ToString());
		}

		[Fact]
		public void VerboseShouldNotWriteToOutputWhenSilent()
		{
			// Arrange
			using var output = new StringWriter();
			var sut = new Logger(output, true, true);

			// Act
			sut.Verbose("test");

			// Assert
			Assert.Empty(output.ToString());
		}

		[Fact]
		public void VerboseShouldWriteToOutputWhenEnabled()
		{
			// Arrange
			using var output = new StringWriter();
			var sut = new Logger(output, false, true);

			// Act
			sut.Verbose("test");

			// Assert
			Assert.Equal("test" + Environment.NewLine, output.ToString());
		}

		[Fact]
		public void WarningShouldNotWriteToOutputWhenSilent()
		{
			// Arrange
			using var output = new StringWriter();
			var sut = new Logger(output, true, false);

			// Act
			sut.Warning("test");

			// Assert
			Assert.Empty(output.ToString());
		}

		[Fact]
		public void WarningShouldWriteToOutputWhenNotSilent()
		{
			// Arrange
			using var output = new StringWriter();
			var sut = new Logger(output, false, false);

			// Act
			sut.Warning("test");

			// Assert
			Assert.Equal("test" + Environment.NewLine, output.ToString());
		}
	}
}