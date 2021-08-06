using System.Threading.Tasks;

namespace Changey.Services
{
	internal interface IExtractor
	{
		Task<bool> Extract(string source, string version, string target, bool header);
	}
}