using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler.Blocks
{
    public class BlockListReference : BlockList<BlockReference>, IDisposable
    {
        protected BlockReference currentBlock1;
        protected override BlockReference currentBlock { get { return currentBlock1; } set { currentBlock1 = value; } }
        public override LinkedList<BlockReference> Blocks { get; set; }
        public override int CurrentBlockLength => (currentBlock.CountBytesInBlock);
        public BlockListReference() : base()
        {
            Blocks = new LinkedList<BlockReference>();
            currentBlock = new BlockReference(0);
        }
        public override void EndCurrentBlock()
        {
            if (CurrentBlockLength > 0)
            {
                Blocks.AddLast(currentBlock);
                currentBlock = new BlockReference(currentBlock.StartBlock + currentBlock.CountBytesInBlock);
                currentBlockStatistic = new Dictionary<byte, uint>();
            }
        }
        public override void CreateNewBlock()
        {
            Blocks.AddLast(currentBlock);
            currentBlock = new BlockReference(currentBlock.StartBlock + currentBlock.CountBytesInBlock);
            currentBlockStatistic = new Dictionary<byte, uint>();
        }
        public override void AddToList(BlockList<BlockReference> blockList)
        {
            EndCurrentBlock();
            blockList.EndCurrentBlock();

            foreach (var item in blockList.Blocks)
            {
                Blocks.AddLast(item);
            }

        }
    }
}
