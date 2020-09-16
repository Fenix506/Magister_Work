using MinFiler.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler
{
    public class BlockSaver
    {
        private string fullDirectoryPath;
        private int blockName;

        public BlockSaver(string absolutDirectoryPath, string fileName)
        {
            fullDirectoryPath = GenerateDirectoryPath(absolutDirectoryPath, fileName);
            CreateDirectory(fullDirectoryPath);
        }
        private string GenerateDirectoryPath(string absolutDirectoryPath, string fileName)
        {
            return Path.Combine(absolutDirectoryPath, fileName);
        }

        private void CreateDirectory(string fullDirectoryPath)
        {
            var directory = new DirectoryInfo(fullDirectoryPath);
            if (!directory.Exists)
            {
                directory.Create();
            }
        }
        private void SaveFileOnDisk(BlocksInList block)
        {
            var FilePath = Path.Combine(fullDirectoryPath, String.Format("{0:d8}", blockName++));

            using (var FileStream = new FileStream(FilePath, FileMode.Create))
            {
                FileStream.Write(block.Data.ToArray(), 0, block.Data.Count);
            }
        }
        private void DeleteFilesFromDisk(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
                Directory.Delete(directoryPath, true);
        }

        public void SaveBlockList(BlockList<BlockReference> blockList, ByteFile file)
        {
            BinaryWriter writer;
            int start;

            foreach (var block in blockList.Blocks)
            {
                start = (int)(block.StartBlock % ByteFile.bufferSize);
                writer = new BinaryWriter(File.Create(fullDirectoryPath + "\\" + blockName++));
                writer.Write(file.Buffer,start,block.CountBytesInBlock);
                writer.Close();
            }

        }
        public void SaveBlockList(BlockList<BlocksInList> blockList)
        {
            BinaryWriter writer;
            foreach (var block in blockList.Blocks)
            {
                writer = new BinaryWriter(File.Create(fullDirectoryPath + "\\" + (blockName++).ToString("{0:d8}")));
                writer.Write(block.Data.ToArray());
                writer.Close();
            }

        }
        public void SaveCompressBlockList(BlockListByteBlock blockList)
        {
            GZipStream zipStream;
            foreach (var block in blockList.Blocks)
            {
                zipStream = new GZipStream(File.Create(fullDirectoryPath + "\\" + blockName++), CompressionLevel.Optimal);
                zipStream.Write(block.Data.ToArray(), 0, block.Data.Count);
                zipStream.Close();
            }
        }
        public void CompressFile(string pathFile)
        {

            using (FileStream sourceStream = new FileStream(pathFile, FileMode.Open))
            {
                // поток для записи сжатого файла
                using (FileStream targetStream = File.Create(fullDirectoryPath + "\\cr"))
                {
                    // поток архивации
                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream); // копируем байты из одного потока в другой

                    }
                }
            }
        }

    }
}
