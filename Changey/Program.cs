﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Changey.Commands;
using CommandLine;

[assembly: InternalsVisibleTo("Changey.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Changey
{
	internal static class Program
	{
		private static async Task Main(string[] args)
		{
			var loader = new CommandLoader();
			var commandTypes = loader.LoadCommandTypes().ToArray();

			await Parser.Default.ParseArguments(args, commandTypes).WithParsedAsync(Run);

			Console.ReadLine();
		}

		private static async Task Run(object arg)
		{
			if (arg is BaseCommand command)
				await command.Execute();
		}
	}
}