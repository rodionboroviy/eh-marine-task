using System.IO;
using System.Threading.Tasks;

namespace MarineTask.Core.IO.Abstractions
{
    public interface IFileReader
    {
        Task<Stream> ReadFile(string path);
    }
}
