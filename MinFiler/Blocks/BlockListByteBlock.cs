using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler.Blocks
{
    public class BlockListByteBlock
    {
        private BlocksInList currentBlock;
        private uint[] fullCountBytes;
        private Dictionary<byte, uint> currentBlockStatistic;
        public LinkedList<BlocksInList> Blocks { get; private set; }
        public int CurrentBlockLength => currentBlock.Data.Count;
        public int CountBlocks => Blocks.Count;
        public BlockListByteBlock()
        {
            Blocks = new LinkedList<BlocksInList>();
            currentBlock = new BlocksInList();
            fullCountBytes = new uint[256];
            currentBlockStatistic = new Dictionary<byte, uint>();
        }
        public static BlockListByteBlock operator +(BlockListByteBlock blockList1, BlockListByteBlock blockList2)
        {

            blockList1.EndCurrentBlock();
            blockList2.EndCurrentBlock();
            foreach (var item in blockList2.Blocks)
            {
                blockList1.Blocks.AddLast(item);
            }
            return blockList1;
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
            currentBlock = new BlocksInList();
            currentBlockStatistic = new Dictionary<byte, uint>();
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
                currentBlock = new BlocksInList();
                currentBlockStatistic = new Dictionary<byte, uint>();
            }

        }
    }
}
