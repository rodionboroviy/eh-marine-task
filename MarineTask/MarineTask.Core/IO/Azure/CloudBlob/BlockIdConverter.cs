using MarineTask.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarineTask.Core.IO.Azure.CloudBlob
{
    public class BlockIdConverter : IBlockIdConverter
    {
        private const char blockIdCharacter = '*';

        public string GenerateBlockIdFromString(string id, int size)
        {
            var stringBuilder = new StringBuilder(id);

            var ammountOfCharsToAdd = size - id.Length;
            if (ammountOfCharsToAdd > 0)
            {
                stringBuilder.Append(blockIdCharacter, ammountOfCharsToAdd);
            }

            return stringBuilder.ToString().ToBase64();
        }

        public string GetStringFromBlockId(string blockId, int size)
        {
            return blockId.FromBase64().Replace(blockIdCharacter.ToString(), string.Empty);
        }
    }
}
