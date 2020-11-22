using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Changey.Options;
using CommandLine;

[assembly: InternalsVisibleTo("Changey.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Changey
{
	[ExcludeFromCodeCoverage]
	internal class Program
	{
		private Program()
		{
			_loader = new TypeLoader();
		}

		private static async Task Main(string[] args)
		{
			await new Program().Run(args);
		}

		private async Task Run(string[] args)
		{
			var commandTypes = _loader.LoadOptionTypes().ToArray();

			await Parser.Default.ParseArguments(args, commandTypes).WithParsedAsync(Run);
		}

		private async Task Run(object arg)
		{
			if (arg is BaseOption option)
			{
				var processor = _loader.FindCommand(option);
				await processor.Execute();
			}
		}

		private readonly TypeLoader _loader;
	}
}