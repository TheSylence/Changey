using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Changey.Tests
{
	internal class IntegrationTestRunner
	{
		public IntegrationTestRunner(Program program)
		{
			_program = program;
		}

		public void AddPass(string command, Func<string, IEnumerable<string>> validator)
		{
			_passes.Add(new IntegrationTestRunPass(command, validator));
		}

		public void AddPass(string command, string[] expectedSubstrings, string[] forbiddenSubstrings)
		{
			var pass = new IntegrationTestRunPass(command, content =>
			{
				var missingSubstrings = expectedSubstrings
					.Where(x => !content.Contains(x))
					.Select(x => $"Missing '{x}' in {content}");

				var forbiddenMatches = forbiddenSubstrings.Where(content.Contains)
					.Select(x => $"Forbidden '{x}' found in {content}");

				var errors = missingSubstrings.Concat(forbiddenMatches).ToList();

				return errors;
			});

			_passes.Add(pass);
		}

		public async Task<IEnumerable<string>> Run(string changeLogPath)
		{
			File.Delete(changeLogPath);
			var errors = new List<string>();

			foreach (var pass in _passes)
			{
				await pass.Execute(_program, changeLogPath);

				var content = await File.ReadAllTextAsync(changeLogPath);

				errors.AddRange(pass.Validate(content));
			}

			return errors;
		}

		private readonly List<IntegrationTestRunPass> _passes = new();
		private readonly Program _program;

		private class IntegrationTestRunPass
		{
			public IntegrationTestRunPass(string command, Func<string, IEnumerable<string>> validator)
			{
				_command = command;
				_validator = validator;
			}

			public async Task Execute(Program program, string changeLogPath)
			{
				var args = SplitCommandArgs(changeLogPath);
				await program.Run(args);
			}

			public IEnumerable<string> Validate(string content) => _validator(content);

			private IEnumerable<string> SplitCommandArgs(string changeLogPath)
			{
				var command = _command.Replace("%changelogpath%", changeLogPath);

				var parts = command.Split(' ');
				var sb = new StringBuilder();
				var inQuotes = false;

				foreach (var part in parts)
				{
					if (part.StartsWith('\"'))
					{
						sb.Append(part.Trim('\"'));
						inQuotes = true;
					}
					else if (part.EndsWith('\"'))
					{
						sb.Append(" ");
						sb.Append(part.Trim('\"'));
						yield return sb.ToString();
						sb.Clear();
						inQuotes = false;
					}
					else
					{
						if (inQuotes)
						{
							sb.Append(" ");
							sb.Append(part);
						}
						else
							yield return part;
					}
				}
			}

			private readonly string _command;
			private readonly Func<string, IEnumerable<string>> _validator;
		}
	}
}