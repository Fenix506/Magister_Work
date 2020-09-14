﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler.Blocks
{
    public class BlockList
    {
        private LinkedList<Block> Blocks;
        private Block currentBlock;
        private uint[] fullCountBytes;
        private Dictionary<byte, byte> currentBlockStatistic;
        public int Position { get; set; }
        public int CurrentBlockLength => currentBlock.CountBlock;
        public int CountBlocks => Blocks.Count;
        public BlockList()
        {
            Blocks = new LinkedList<Block>();
            currentBlock = new Block();
            fullCountBytes = new uint[256];
            currentBlockStatistic = new Dictionary<byte, byte>();
        }
        public static BlockList operator +(BlockList blockList1, BlockList blockList2) {
            var blockList = new BlockList();
            blockList.Blocks = blockList1.Blocks;
            foreach(var item in blockList2.Blocks)
            {
                blockList.Blocks.AddLast(item);
            }
            return blockList;
        }
        public void AddToBlock(byte currentByte)
        {
            currentBlock.Data.Add(currentByte);
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
        public void CreateNewBlock()
        {
            Blocks.AddLast(currentBlock);
            currentBlock = new Block();
            currentBlockStatistic = new Dictionary<byte, byte>();
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

        public void EndCurrentBlock()
        {
            if (CurrentBlockLength > 0)
            {
                Blocks.AddLast(currentBlock);
                currentBlock = new Block();
                currentBlockStatistic = new Dictionary<byte, byte>();
            }
           
        }
    }
}
