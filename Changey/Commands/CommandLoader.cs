using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLine;

namespace Changey.Commands
{
	internal class CommandLoader
	{
		public IEnumerable<Type> LoadCommandTypes()
		{
			var executingAssembly = Assembly.GetExecutingAssembly();
			var exportedTypes = executingAssembly.GetTypes();

			return exportedTypes.Where(t => !t.IsAbstract && t.GetCustomAttribute<VerbAttribute>() != null);
		}
	}
}