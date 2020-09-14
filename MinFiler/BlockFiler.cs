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
        private ByteFile file;
        
        public double FileEntropy { get; set; }
        public BlockList BlockList { get; set; }
        public BlockFiler(string fullFileName, double blockEntropy = 4)
        {
            this.blockEntropy = blockEntropy;
            BlockList = new BlockList();
            file = new ByteFile(fullFileName);

        }
        public BlockFiler(string fullFileName, Action AddProgress, double blockEntropy = 4)
            : this(fullFileName, blockEntropy)
        {
            file = new ByteFile(fullFileName, AddProgress);
        }
        public void Bloking()
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
        public async Task BlokingAsync()
        {
            await Task.Run(() => Bloking());
        }
        public async Task ParallelBlokingAsync()
        {
            await Task.Run(() => ParralelBloking());
        }
        public void ParralelBloking()
        {
            var rangePartitioner = Partitioner.Create(0, file.BufferArraySize, file.BufferArraySize / file.CountThreads);
            var partBlockList = new BlockList[file.CountThreads];

            for (int part = 0; part < file.CountPartReadFile; part++)
            {
                Parallel.ForEach(rangePartitioner,

                (range, loopState, threadNum) =>
                  {
                      partBlockList[threadNum] = new BlockList();

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
                    BlockList += partBlockList[i];
                }
                file.ReadNewPartFile();
            }

            Task<BlockList> EndOfFileTask = new Task<BlockList>(() => BlokingEndOfFile());
            EndOfFileTask.Start();
            EndOfFileTask.Wait();
            BlockList = BlockList + EndOfFileTask.Result;
        }
        public BlockList BlokingEndOfFile()
        {
            byte readByte;
            double currentEntropy;
            var blockList = new BlockList();

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
