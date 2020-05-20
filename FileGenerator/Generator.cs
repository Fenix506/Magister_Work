using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileGenerator
{
    public class Generator
    {
        private ulong size;
        private string name;
        private string path;
        private Random random;
        public Generator(ulong sizeByte)
        {
            this.size = sizeByte;
            this.name = DateTime.Now.ToString("HHmmss");
            this.path = Directory.CreateDirectory("./Files").FullName;
           
            random = new Random();
        }
        public Generator(ulong sizeByte, string name) : this(sizeByte)
        {
            this.name = name;
        }
        public Generator(ulong sizeByte, string name, string path) : this(sizeByte, name)
        {
            this.path = path;
        }

        public void Generate()
        {
            var fullPath = Path.Combine(path, name);
            ulong bufferSize = 104857600; //100Mb
            using (var binaryWriter = new BinaryWriter(File.Create(fullPath, 512, FileOptions.SequentialScan)))
            {
                byte[] buffervalue = new byte[bufferSize];
                int countGenerate = (int)(size / bufferSize);
                for (int i = 0; i < countGenerate; i++)
                {
                    random.NextBytes(buffervalue);

                    binaryWriter.Write(buffervalue);
                }
                if (size % bufferSize != 0)
                {
                    buffervalue = new byte[size % bufferSize];
                    random.NextBytes(buffervalue);
                    binaryWriter.Write(buffervalue);
                }
            }
        }
    }
}
