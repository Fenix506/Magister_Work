using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler.Blocks
{
    public class BlockListReference : BlockList
    {
        // protected BlockReference currentBlock1;
        protected override IBlock currentBlock { get; set; }
        public override LinkedList<IBlock> Blocks { get; set; }
        public BlockListReference() : base()
        {
            Blocks = new LinkedList<IBlock>();
            currentBlock = new BlockReference();
        }
        public override void EndCurrentBlock()
        {
            if (CurrentBlockLength > 0)
            {
                Blocks.AddLast(currentBlock);
                currentBlock = new BlockReference(currentBlock.GetFirst + currentBlock.CountBytesInBlock);
                currentBlockStatistic = new Dictionary<byte, uint>();
            }
        }
        public override void CreateNewBlock()
        {
            Blocks.AddLast(currentBlock);
            currentBlock = new BlockReference(currentBlock.GetFirst + currentBlock.CountBytesInBlock);
            currentBlockStatistic = new Dictionary<byte, uint>();
        }

    }
}
