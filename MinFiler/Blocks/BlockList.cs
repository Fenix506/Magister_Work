﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler.Blocks
{
    public abstract class BlockList<T> where T : IBlock
    {
        protected uint[] fullCountBytes;
        protected Dictionary<byte, uint> currentBlockStatistic;
        abstract protected T currentBlock { get; set; }
        abstract public LinkedList<T> Blocks { get; set; }
        abstract public int CurrentBlockLength { get; }
        public int CountBlocks => Blocks.Count;
        public BlockList()
        {
            fullCountBytes = new uint[256];
            currentBlockStatistic = new Dictionary<byte, uint>();
        }
        abstract public void AddToList(BlockList<T> blockList);
        abstract public void EndCurrentBlock();
        abstract public void CreateNewBlock();
        public void AddToBlock(byte currentByte)
        {
            currentBlock.Add(currentByte);
            fullCountBytes[currentByte]++;
            if (currentBlockStatistic.ContainsKey(currentByte))
            {
                currentBlockStatistic[currentByte]++;
            }
            else
            {
                currentBlockStatistic.Add(currentByte, 1);
            }
        }
        public double CurrentBlockEntropy()
        {
            uint length = 0;
            double entropy = 0;

            foreach (var item in currentBlockStatistic)
            {
                checked
                {
                    length += item.Value;
                }
            }
            foreach (var item in currentBlockStatistic)
            {

                double pi = (double)item.Value / length;

                checked { entropy += pi * Math.Log(pi, 2); }

            }

            return -entropy;
        }
        public double FullEntropy()
        {
            uint length = 0;
            double entropy = 0;
            for (int i = 0; i < 256; i++)
            {
                checked
                {
                    length += fullCountBytes[i];
                }
            }
            for (int i = 0; i < 256; i++)
            {
                if (fullCountBytes[i] > 0)
                {
                    double pi = (double)fullCountBytes[i] / length;

                    checked { entropy += pi * Math.Log(pi, 2); }
                }
            }
            return -entropy;
        }
        public void Dispose()
        {
            fullCountBytes = null;
            currentBlockStatistic = null;
            Blocks = null;
            GC.Collect(2, GCCollectionMode.Forced);
        }
    }
}
