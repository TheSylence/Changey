using System;
using System.IO;

namespace Changey;

internal class Logger : ILogger
{
	public Logger(TextWriter output, bool silent, bool verbose)
	{
		_output = output;
		_silent = silent;
		_verbose = verbose;
	}

	public void Error(string message)
	{
		if (!_silent)
			_output.WriteLine(message);
	}

	public void Error(string message, Exception exception)
	{
		if (_silent)
			return;

		_output.WriteLine(message);

		if (_verbose)
			_output.WriteLine(exception);
	}

	public void Info(string message)
	{
		if (!_silent)
			_output.WriteLine(message);
	}

	public void Verbose(string message)
	{
		if (_verbose && !_silent)
			_output.WriteLine(message);
	}

	public void Warning(string message)
	{
		if (!_silent)
			_output.WriteLine(message);
	}

	private readonly TextWriter _output;
	private readonly bool _silent;
	private readonly bool _verbose;
}