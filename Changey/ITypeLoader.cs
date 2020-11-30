using System;
using System.Collections.Generic;
using Changey.Commands;
using Changey.Options;

namespace Changey
{
	internal interface ITypeLoader
	{
		ICommand FindCommand(BaseOption option);
		IEnumerable<Type> LoadOptionTypes();
	}
}