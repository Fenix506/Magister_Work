using MinFiler.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler
{
    public class BlockSaver
    {
        private string directoryPath;
        private string fileName;
        private BlockList blockList;

        public BlockSaver(string directoryPath,string fileName)
        {
            this.directoryPath = directoryPath;
            this.fileName = fileName;
        }

    }
}
