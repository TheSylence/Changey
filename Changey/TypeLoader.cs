using System;
using System.Collections.Generic;
using Changey.Commands;
using Changey.Options;
using Changey.Services;

namespace Changey
{
	internal class TypeLoader
		: ITypeLoader
	{
		public TypeLoader()
		{
			var fileAccess = new FileAccess();
			_changeLogSerializer = new ChangeLogSerializer(fileAccess);
		}

		public ICommand FindCommand(BaseOption option)
		{
			return option switch
			{
				InitOption i => Init(i),
				SectionOption s => Section(s),
				YankOption y => Yank(y),
				ReleaseOption r => Release(r),
				ExtractOption e => Extract(e),
				_ => throw new ArgumentException($"Failed to find command for {option.GetType()}")
			};
		}

		public IEnumerable<Type> LoadOptionTypes()
		{
			yield return typeof(InitOption);
			yield return typeof(ReleaseOption);
			yield return typeof(YankOption);
			yield return typeof(AddOption);
			yield return typeof(ChangeOption);
			yield return typeof(DeprecatedOption);
			yield return typeof(FixOption);
			yield return typeof(RemoveOption);
			yield return typeof(SecurityOption);
			yield return typeof(ExtractOption);
		}

		private ICommand Extract(ExtractOption extractOption)
		{
			var extractor = new Extractor(extractOption.Logger, _changeLogSerializer);
			return new ExtractCommand(extractOption, extractor);
		}

		private ICommand Init(InitOption option)
		{
			var changeLogCreator = new ChangeLogCreator(option.Logger, _changeLogSerializer);
			return new InitCommand(option, changeLogCreator);
		}

		private ICommand Release(ReleaseOption releaseOption)
		{
			var changeLogReleaser = new ChangeLogReleaser(releaseOption.Logger, _changeLogSerializer);
			return new ReleaseCommand(releaseOption, changeLogReleaser);
		}

		private ICommand Section(SectionOption sectionOption)
		{
			var sectionAdder = new SectionAdder(_changeLogSerializer, sectionOption.Logger);
			return new SectionCommand(sectionOption, sectionAdder);
		}

		private ICommand Yank(YankOption yankOption)
		{
			var versionYanker = new VersionYanker(yankOption.Logger, _changeLogSerializer);
			return new YankCommand(yankOption, versionYanker);
		}

		private readonly ChangeLogSerializer _changeLogSerializer;
	}
}