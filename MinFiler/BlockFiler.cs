using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinFiler.Blocks;

namespace MinFiler
{
    public class BlockFiler
    {
        private string fullFileName;
        private double blockEntropy;
       
        public double FileEntropy { get; set; }
        public BlockList BlockList { get; set; }
        public event Action AddProgress;
        public event Action Finish;
        public BlockFiler(string fullFileName, double blockEntropy = 4)
        {
            this.fullFileName = fullFileName;
            this.blockEntropy = blockEntropy;
            BlockList = new BlockList();

        }
        public void Bloking()
        {
            var file = new ByteFile(fullFileName, AddProgress, Finish);

            byte readByte;
            double currentEntropy;
            while (!file.isEnd())
            {
                readByte = file.getByte();

                if (BlockList.CurrentBlockLength <= 4)
                {
                    currentEntropy = 0;
                }
                else
                {
                    currentEntropy = BlockList.CurrentBlockEntropy();
                }
                if (currentEntropy < blockEntropy)
                {
                    BlockList.AddToBlock(readByte);
                }
                else
                {
                    BlockList.CreateNewBlock();
                    BlockList.AddToBlock(readByte);
                }
            }
            FileEntropy = BlockList.FullEntropy();
        }
        public async Task BlokingAsync()
        {
            await Task.Run(() => Bloking());
        }

    }
}
