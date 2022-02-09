using MarineTask.ValidationApp.Processors.Result;

namespace MarineTask.ValidationApp.Processors
{
    internal interface IRecordLineProcessor<TResult>
        where TResult : class
    {
        void ProcessLine(string line);

        ProcessResult<TResult> GetResult();
    }
}
