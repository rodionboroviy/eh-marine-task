using MarineTask.ValidationApp.Processors.Result;
using System.Threading.Tasks;

namespace MarineTask.ValidationApp.Processors
{
    internal interface IRecordLineProcessor<TResult>
        where TResult : class
    {
        Task ProcessLine(string line);

        Task<ProcessResult<TResult>> GetResult();
    }
}
