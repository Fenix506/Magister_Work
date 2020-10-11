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
        private string fileName;
        private int blockName;
        private Deflate deflate;

        public BlockSaver(string absolutDirectoryPath, string fileName)
        {
            fullFileName = fileName;
            this.fileName = new FileInfo(fullFileName).Name;
            fullDirectoryPath = GenerateDirectoryPath(absolutDirectoryPath, this.fileName);
            CreateDirectory(fullDirectoryPath);
            deflate = new Deflate();
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
            int start;

            foreach (var block in blockList.Blocks)
            {
                start = (int)(block.GetFirst % ByteFile.bufferSize);

                deflate.CompressBlock(fullDirectoryPath + "\\" + String.Format("{0:d8}", blockName),
                    file.Buffer,
                    start,
                    block.CountBytesInBlock);

                blockName++;
            }
        }
        public void SaveCompressBlockList(BlockList blockList)
        {

            foreach (var block in blockList.Blocks)
            {
                deflate.CompressBlock(fullDirectoryPath + "\\" + String.Format("{0:d8}", blockName),
                    block.GetBlock,
                    0,
                    block.CountBytesInBlock);
                blockName++;
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
        public void CompressFile()
        {
            using (FileStream sourceStream = new FileStream(fullFileName, FileMode.Open))
            {
                var targetNamePath = Path.Combine(fullDirectoryPath, fileName);
                deflate.CompressFile(sourceStream, targetNamePath);
            }
        }
        public void DecompressFile()
        {
            using (FileStream sourceStream = new FileStream(fullFileName, FileMode.Open))
            {
                var targetNamePath = Path.Combine(fullDirectoryPath, fileName);
                deflate.DecompressFile(sourceStream, targetNamePath);
            }

        }
        public void CompressBlocksFile()
        {
            var files = Directory.GetFiles(fullFileName);
            foreach(var filePath in files)
            {
                using (var file = new FileStream(filePath,FileMode.Open))
                {
                    var targetNamePath = Path.Combine(fullDirectoryPath, String.Format("{0:d8}",blockName));
                    blockName++;
                    deflate.CompressFile(file, targetNamePath);
                }
            }
        }
        public void DecompressBloksFile()
        {
            var files = Directory.GetFiles(fullFileName);
            foreach (var filePath in files)
            {
                using (var file = new FileStream(filePath, FileMode.Open))
                {
                    var targetNamePath = Path.Combine(fullDirectoryPath, String.Format("{0:d8}", blockName));
                    blockName++;
                    deflate.DecompressFile(file, targetNamePath);
                }
            }
        }
    }
}
