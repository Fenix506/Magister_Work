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
        private string fullFileName;
        private int blockName;

        public BlockSaver(string absolutDirectoryPath, string fileName)
        {
            fullFileName = fileName;
            fullDirectoryPath = GenerateDirectoryPath(absolutDirectoryPath, new FileInfo(fullFileName).Name);
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
        public void SaveBlockList(BlockList blockList, ByteFile file)
        {
            BinaryWriter writer;
            int start;

            foreach (var block in blockList.Blocks)
            {
                start = (int)(block.GetFirst % ByteFile.bufferSize);
                writer = new BinaryWriter(File.Create(fullDirectoryPath + "\\" + String.Format("{0:d8}", blockName)));
                blockName++;
                writer.Write(file.Buffer, start, block.CountBytesInBlock);
                writer.Close();
            }
        }
        public void SaveBlockList(BlockList blockList)
        {
            BinaryWriter writer;
            foreach (var block in blockList.Blocks)
            {
                writer = new BinaryWriter(File.Create(fullDirectoryPath + "\\" + String.Format("{0:d8}", blockName)));
                blockName++;
                writer.Write(block.GetBlock);
                writer.Close();
            }

        }
        public void SaveCompressBlockList(BlockList blockList, ByteFile file)
        {
            GZipStream zipStream;
            int start;

            foreach (var block in blockList.Blocks)
            {
                start = (int)(block.GetFirst % ByteFile.bufferSize);
                zipStream = new GZipStream(File.Create(fullDirectoryPath + "\\" + String.Format("{0:d8}", blockName)), CompressionLevel.Optimal);
                blockName++;
                zipStream.Write(file.Buffer, start, block.CountBytesInBlock);
                zipStream.Close();
            }
        }
        public void SaveCompressBlockList(BlockList blockList)
        {
            GZipStream zipStream;
            foreach (var block in blockList.Blocks)
            {
                zipStream = new GZipStream(File.Create(fullDirectoryPath + "\\" + String.Format("{0:d8}", blockName)), CompressionLevel.Optimal);
                blockName++;
                zipStream.Write(block.GetBlock, 0, block.CountBytesInBlock);
                zipStream.Close();
            }
        }
        public void SaveBlockedFile(BlockList blockList)
        {
            switch (blockList)
            {
                case BlockListReference blockListReference:
                    SaveReferenceBlocks(blockListReference);
                    break;
                case BlockListByteBlock blockListByteBlock:
                    SaveBlockList(blockListByteBlock);
                    break;
                default:
                    throw new InvalidCastException();
            }
        }

        private void SaveReferenceBlocks(BlockListReference blockList)
        {
            var file = new ByteFile(fullFileName);

            BinaryWriter writer;
            int start;

            foreach (var block in blockList.Blocks)
            {
                if (block.GetFirst >= ByteFile.bufferSize)
                {
                    file.ReadNewPartFile();
                }
                start = (int)(block.GetFirst % ByteFile.bufferSize);
                writer = new BinaryWriter(File.Create(fullDirectoryPath + "\\" + String.Format("{0:d8}", blockName)));
                blockName++;
                writer.Write(file.Buffer, start, block.CountBytesInBlock);
                writer.Close();
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
