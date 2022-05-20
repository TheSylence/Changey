using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Changey.Options;
using CommandLine;

[assembly: InternalsVisibleTo("Changey.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Changey;

internal class Program
{
	[ExcludeFromCodeCoverage]
	private Program()
		: this(null, null)
	{
	}

	internal Program(ITypeLoader? typeLoader, TextWriter? helpWriter)
	{
		_loader = typeLoader ?? new TypeLoader();
		_helpWriter = helpWriter ?? Console.Out;
	}

	internal async Task Run(IEnumerable<string> args)
	{
		var commandTypes = _loader.LoadOptionTypes().ToArray();

		var parser = new Parser(configuration =>
		{
			configuration.HelpWriter = _helpWriter;
			configuration.AutoHelp = true;
			configuration.AutoVersion = true;
		});

		await parser.ParseArguments(args, commandTypes).WithParsedAsync(Run);
	}

	[ExcludeFromCodeCoverage]
	private static async Task Main(string[] args)
	{
		await new Program().Run(args);
	}

	private async Task Run(object arg)
	{
		if (arg is BaseOption option)
		{
			var processor = _loader.FindCommand(option);
			await processor.Execute();
		}
	}

	private readonly ITypeLoader _loader;
	private readonly TextWriter _helpWriter;
}