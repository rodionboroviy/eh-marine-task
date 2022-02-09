namespace MarineTask.ValidationApp.Processors.Result
{
    public class ProcessResult<TResult>
        where TResult : class
    {
        public TResult Result { get; set; }
    }
}
