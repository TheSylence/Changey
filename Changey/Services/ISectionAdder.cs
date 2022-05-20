using System.Threading.Tasks;
using Changey.Models;

namespace Changey.Services;

internal interface ISectionAdder
{
	Task AddToSection(string path, Section section, string message);
}