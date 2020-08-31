using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler.Blocks
{
    public class Block
    {
        public LinkedList<byte> Data { get; set; } = new LinkedList<byte>(10);
        public int CountBlock => Data.Count();
        public double Entropy { get; set; }

    }
}
