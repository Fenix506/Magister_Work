using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler.Blocks
{
    public class BlockReference : IBlock
    {
        public long StartBlock;
        public int CountBytes;
        public BlockReference(long StartBlock)
        {
            this.StartBlock = StartBlock;
            CountBytes = 0;
        }

        public int CountBytesInBlock => CountBytes;

        public void Add(byte addByte)
        {
            CountBytes++;
        }
    }
}
