using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CommandLine.Text;

namespace Changey.Options;

internal static class ExampleBuilder
{
	internal static IEnumerable<Example> ExamplesFor<TSectionOption>()
		where TSectionOption : SectionOption
	{
		var ctor = typeof(TSectionOption).GetConstructors().FirstOrDefault();
		Debug.Assert(ctor != null, nameof(ctor) + " != null");

		var examples = new Dictionary<string, string>
		{
			{ "Add sentence as message to section", "Hello, World!" }
		};

		foreach (var (helpText, message) in examples)
		{
			var obj = ctor.Invoke(new object[] { message, string.Empty, false, false });
			yield return new Example(helpText, obj);
		}
	}
}