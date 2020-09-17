using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler.Blocks
{
    public interface IBlock
    {
        void Add(byte addByte);
        long GetFirst { get; }
        byte[] GetBlock { get; }
        int CountBytesInBlock { get; }
    }
}
