using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler.Blocks
{
    public abstract class BlockList : IDisposable
    {
        protected uint[] fullCountBytes;
        protected Dictionary<byte, uint> currentBlockStatistic;
        abstract protected IBlock currentBlock { get; set; }
        abstract public LinkedList<IBlock> Blocks { get; set; }
        public int CountBlocks => Blocks.Count;
        public int CurrentBlockLength => currentBlock.CountBytesInBlock;
        
        public BlockList()
        {
            fullCountBytes = new uint[256];
            currentBlockStatistic = new Dictionary<byte, uint>();
        }
        abstract public void EndCurrentBlock();
        abstract public void CreateNewBlock();
        public void AddToList(BlockList blockList)
        {
            {
                EndCurrentBlock();
                blockList.EndCurrentBlock();

                foreach (var item in blockList.Blocks)
                {
                    Blocks.AddLast(item);
                }
                for (int i = 0; i < blockList.fullCountBytes.Length; i++)
                {
                    fullCountBytes[i] += blockList.fullCountBytes[i];
                }
                blockList.Dispose();
            }
        }
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
         
            double entropy = 0;

          
            foreach (var item in currentBlockStatistic)
            {

                double pi = (double)item.Value / currentBlock.CountBytesInBlock;

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
