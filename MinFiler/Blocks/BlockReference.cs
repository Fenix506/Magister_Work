using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler.Blocks
{
    public struct BlockReference : IBlock
    {
        private long StartBlock;
        public int CountBytes;
        public BlockReference(long StartBlock)
        {
            this.StartBlock = StartBlock;
            CountBytes = 0;
        }
        public byte[] GetBlock => throw new NotImplementedException();
        public int CountBytesInBlock => CountBytes;
        public long GetFirst => StartBlock;
        public void Add(byte addByte)
        {
            CountBytes++;
        }

    }
}
