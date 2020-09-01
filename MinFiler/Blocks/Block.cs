using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler.Blocks
{
    public class Block
    {
        public List<byte> Data { get; set; } = new List<byte>(20);
        public int CountBlock => Data.Count();
        public double Entropy { get; set; }

    }
}
