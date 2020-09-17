using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler.Blocks
{
    public class BlockListByteBlock : BlockList
    {
        protected override IBlock currentBlock { get; set; }
        public override LinkedList<IBlock> Blocks { get; set; }
        public BlockListByteBlock()
        {
            Blocks = new LinkedList<IBlock>();
            currentBlock = new BlocksInList();
        }
        public override void EndCurrentBlock()
        {
            if (CurrentBlockLength > 0)
            {
                Blocks.AddLast(currentBlock);
                currentBlock = new BlocksInList();
                currentBlockStatistic = new Dictionary<byte, uint>();
            }
        }

        public override void CreateNewBlock()
        {
            Blocks.AddLast(currentBlock);
            currentBlock = new BlocksInList();
            currentBlockStatistic = new Dictionary<byte, uint>();
        }
    }
}
