using System;
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
        public BlockList()
        {
            Blocks = new LinkedList<Block>();
            currentBlock = new Block();
        }
        public void AddToBlock(byte currentByte)
        {
            currentBlock.Data.Add(currentByte);
        }
        public void CreateNewBlock()
        {
            Blocks.AddLast(currentBlock);
            currentBlock = new Block();
        }
        public int CurrentBlockLength => currentBlock.CountBlock;
    }
}
