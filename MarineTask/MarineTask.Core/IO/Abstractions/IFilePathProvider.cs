namespace MarineTask.Core.IO.Abstractions
{
    public interface IFilePathProvider
    {
        string GetImageFilePath(string fileName);

        string GetRecordFilePath(string fileName);
    }
}
