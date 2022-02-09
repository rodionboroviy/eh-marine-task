using System.IO;
using System.Threading.Tasks;

namespace MarineTask.Core.IO.Abstractions
{
    public interface IFileWriter
    {
        Task<string> WriteFile(Stream contentToWrite, string path, int blockNumber, int blockCount);
    }
}
