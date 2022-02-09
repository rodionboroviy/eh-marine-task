using System.IO;
using System.Threading.Tasks;

namespace MarineTask.EventSender
{
    internal interface IUploadManager
    {
        Task SendStreamInChuncks(Stream stream, string name, int pageSizeInBytes = 50_000);
    }
}
