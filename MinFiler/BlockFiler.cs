using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MinFiler.Blocks;

namespace MinFiler
{
    public class BlockFiler
    {
        private double blockEntropy;
        public string FullFileName { get; set; }
        private ByteFile file;
        private BlockSaver saver;
        private Task saveTask;
        public double FileEntropy { get; set; }
        public BlockList BlockList { get; set; }
        public BlockFiler(string fullFileName, double blockEntropy = 4)
        {
            this.blockEntropy = blockEntropy;
            BlockList = new BlockListReference();
            file = new ByteFile(fullFileName);
            FullFileName = fullFileName;
            saver = new BlockSaver("D:\\Blocks", fullFileName);
        }
        public BlockFiler(string fullFileName, Action AddProgress, double blockEntropy = 4)
            : this(fullFileName, blockEntropy)
        {
            if (file != null)
            {
                file.Dispose();
            }
            file = new ByteFile(fullFileName, AddProgress);
        }
        public async Task BlokingAsync(bool save)
        {
            await Task.Run(() => Bloking(save));
        }
        public async Task ParallelBlokingAsync(bool save)
        {
            await Task.Run(() => ParralelBloking(save, false));
        }
        public async Task BlokingWithCompressAsync(bool save)
        {
            await Task.Run(() => BlokingWithCompress(save));
        }
        public void Bloking(bool save)
        {
            byte readByte;
            double currentEntropy;
            while (!file.isEnd())
            {
                readByte = file.GetByte();

                if (BlockList.CurrentBlockLength <= 4)
                {
                    currentEntropy = 0;
                }
                else
                {
                    currentEntropy = BlockList.CurrentBlockEntropy();
                }
                if (currentEntropy < blockEntropy)
                {
                    BlockList.AddToBlock(readByte);
                }
                else
                {
                    BlockList.CreateNewBlock();
                    BlockList.AddToBlock(readByte);
                }
            }
            BlockList.EndCurrentBlock();

            FileEntropy = BlockList.FullEntropy();
        }
        public void ParralelBloking(bool save, bool compress)
        {
            var rangePartitioner = Partitioner.Create(0, file.BufferArraySize, file.BufferArraySize / file.CountThreads);
            var partBlockList = new BlockList[file.CountThreads];

            for (int part = 0; part < file.CountPartReadFile; part++)
            {
                Parallel.ForEach(rangePartitioner,

                (range, loopState, threadNum) =>
                  {

                      partBlockList[threadNum] = new BlockListReference();

                      for (int i = range.Item1; i < range.Item2; i++)
                      {
                          if (partBlockList[threadNum].CurrentBlockLength <= 4)
                          {
                              partBlockList[threadNum].AddToBlock(file.GetByte(i));
                              continue;
                          }

                          if (partBlockList[threadNum].CurrentBlockEntropy() < blockEntropy)
                          {
                              partBlockList[threadNum].AddToBlock(file.GetByte(i));
                          }
                          else
                          {
                              partBlockList[threadNum].CreateNewBlock();
                              partBlockList[threadNum].AddToBlock(file.GetByte(i));
                          }
                      }
                      partBlockList[threadNum].EndCurrentBlock();

                  });
                for (int i = 0; i < partBlockList.Length; i++)
                {
                    BlockList.AddToList(partBlockList[i]);
                }
                if (save) SaveBlock(compress);
                file.ReadNewPartFile();

            }

            Task<BlockList> EndOfFileTask = new Task<BlockList>(() => BlokingEndOfFile());
            EndOfFileTask.Start();
            EndOfFileTask.Wait();
            BlockList.AddToList(EndOfFileTask.Result);
            FileEntropy = BlockList.FullEntropy();

            if (save) SaveBlock(compress);

        }
        public void BlokingWithCompress(bool save)
        {
            ParralelBloking(save, true);
        }
        public void SaveBlock(bool compress)
        {
            if (compress)
            {
                saveTask = new Task(() => saver.SaveCompressBlockList(BlockList, file));
            }
            else
            {
                switch (BlockList)
                {
                    case BlockListReference blockListReference:
                        saveTask = new Task(() => saver.SaveBlockList(BlockList, file));
                        break;
                    case BlockListByteBlock blockListByteBlock:
                        saveTask = new Task(() => saver.SaveBlockList(BlockList));
                        break;
                    default:
                        throw new InvalidCastException();
                }
            }

            saveTask.Start();
            saveTask.Wait();
            BlockList.Dispose();
            BlockList = new BlockListReference();
        }
        private BlockList BlokingEndOfFile()
        {
            byte readByte;
            double currentEntropy;
            var blockList = new BlockListReference();

            for (int i = 0; i < file.BufferArraySize; i++)
            {
                readByte = file.GetByte(i);

                if (blockList.CurrentBlockLength <= 4)
                {
                    blockList.AddToBlock(readByte);
                    continue;
                }

                currentEntropy = blockList.CurrentBlockEntropy();

                if (currentEntropy < blockEntropy)
                {
                    blockList.AddToBlock(readByte);
                }
                else
                {
                    blockList.CreateNewBlock();
                    blockList.AddToBlock(readByte);
                }
            }
            blockList.EndCurrentBlock();
            return blockList;
        }
        
    }
}
