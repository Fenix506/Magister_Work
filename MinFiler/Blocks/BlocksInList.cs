using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler.Blocks
{
    public class BlocksInList : IBlock
    {
        public List<byte> Data { get; set; } = new List<byte>(16);

        public int CountBytesInBlock => Data.Count;

        public long GetFirst => Data.First();

        byte[] IBlock.GetBlock => Data.ToArray();

        public void Add(byte addByte)
        {
            Data.Add(addByte);
        }


    }
}
