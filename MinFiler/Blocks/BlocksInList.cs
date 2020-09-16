using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler.Blocks
{
    public class BlocksInList:IBlock
    {
        public List<byte> Data { get; set; } = new List<byte>(16);

        public int CountBytesInBlock => Data.Count;

        public void Add(byte addByte)
        {
            Data.Add(addByte);
        }
    }
}
