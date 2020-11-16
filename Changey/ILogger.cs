using System;

namespace Changey
{
	internal interface ILogger
	{
		void Error(string message);
		void Error(string message, Exception exception);
		void Info(string message);
		void Verbose(string message);
		void Warning(string message);
	}
}