using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler
{
    public class Deflate
    {
        private GZipStream zipStream;
        public Deflate()
        {

        }
        public void CompressBlock(string fullFilePath, byte[] buffer, int start, int countBytes)
        {
            using (var fileStream = File.Create(fullFilePath))
            {
                zipStream = new GZipStream(fileStream, CompressionLevel.Optimal);

                zipStream.Write(buffer, start, countBytes);
                zipStream.Close();

            }

        }
        public void CompressFile(Stream sourceStream, string fullTargetPath)
        {
            using (FileStream targetStream = File.Create(fullTargetPath))
            {
                using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                {
                    sourceStream.CopyTo(compressionStream);
                }
            }
        }
        public void DecompressFile(Stream sourseStream, string fullTargetPath)
        {
            using (FileStream targetStream = File.Create(fullTargetPath))
            {
                using (GZipStream decompressionStream = new GZipStream(sourseStream, CompressionMode.Decompress))
                {
                    decompressionStream.CopyTo(targetStream);
                }
            }
        }
    }
}
