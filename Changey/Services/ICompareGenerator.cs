using System.Threading.Tasks;

namespace Changey.Services;

internal interface ICompareGenerator
{
    Task<bool> Generate(string path, string baseUrl, string compareTemplate, string releaseTemplate);
}