using Azure.Storage.Blobs.Specialized;
using MarineTask.Configuration;
using MarineTask.Core.IO.Azure.CloudBlob;
using MarineTask.ValidationApp.Extensions;
using MarineTask.ValidationApp.Processors.Result;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;

namespace MarineTask.ValidationApp.Processors
{
    public class SequenceLineProcessor : IRecordLineProcessor<SequenceResult>
    {
        private string recordId;
        private List<string> addedBlockIds = new List<string>(0);

        private BlockBlobClient blockClient;

        private readonly ICloudBlobClientResolver cloudBlobClientResolver;
        private readonly IBlockIdConverter blockIdConverter;

        private readonly AppConfiguration config;

        public SequenceLineProcessor(IOptions<AppConfiguration> config)
        {
            this.cloudBlobClientResolver = new CloudBlobClientResolver();
            this.blockIdConverter = new BlockIdConverter();

            this.config = config.Value;
        }

        public void ProcessLine(string line)
        {
            if (line.IsRecordId())
            {
                var blobConnectionString = this.config.ConnectionStrings.FileStore.ConnectionString;
                var blobContainer = this.config.ConnectionStrings.FileStore.Container;

                recordId = line.Replace("RecordID:", string.Empty).Trim();

                var filePath = $"{blobContainer}/{recordId}.txt";

                this.blockClient = this.cloudBlobClientResolver.GetCloudBlockBlobClient(filePath, blobConnectionString);
            }

            if (line.IsSequenceLine())
            {
                var blockId = this.blockIdConverter.GenerateBlockIdFromString(line, 25);
                addedBlockIds.Add(blockId);

                using (var stream = new MemoryStream())
                {
                    using (var sw = new StreamWriter(stream))
                    {
                        sw.WriteLine(line);
                        sw.Flush();
                        stream.Seek(0, SeekOrigin.Begin);

                        this.blockClient.StageBlock(blockId, stream);
                    }
                }
            }
        }

        public ProcessResult<SequenceResult> GetResult()
        {
            this.blockClient.CommitBlockList(addedBlockIds);

            return new ProcessResult<SequenceResult>
            {
                Result = new SequenceResult
                {
                    RecordId = this.recordId,
                    FileUrl = this.blockClient.Uri.ToString()
                }
            };
        }
    }
}
