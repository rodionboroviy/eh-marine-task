namespace MarineTask.Core.IO.Azure.CloudBlob
{
    public interface IBlockIdConverter
    {
        string GenerateBlockIdFromString(string id, int size);

        string GetStringFromBlockId(string blockId, int size);
    }
}
