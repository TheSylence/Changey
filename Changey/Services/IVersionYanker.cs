using System.Threading.Tasks;

namespace Changey.Services;

internal interface IVersionYanker
{
	Task<bool> Yank(string path);
}