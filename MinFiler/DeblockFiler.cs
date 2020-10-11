using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler
{
    public class DeblockFiler
    {
        string BlockPath;
        string WhereSave;
        public DeblockFiler(string BlockPath, string whereSave)
        {
            this.BlockPath = BlockPath;
            this.WhereSave = whereSave;
        }
        public void Deblocking()
        {
            var files = Directory.GetFiles(BlockPath);
            var deblockFilePath = Path.Combine(WhereSave, new FileInfo(BlockPath).Name);
            var deblockFile = new FileStream(deblockFilePath, FileMode.Create);
            
            foreach (var filePath in files)
            {
                using (var block = new FileStream(filePath, FileMode.Open))
                {
                    block.CopyTo(deblockFile);
                }
            }
            deblockFile.Close();
        }

    }
}
