using System.Threading.Tasks;

namespace Changey.Commands
{
	internal interface ICommand
	{
		Task Execute();
	}
}