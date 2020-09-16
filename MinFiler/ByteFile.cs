using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler
{
    public class ByteFile : IDisposable
    {

        public const int bufferSize = 52428800;//104857600 7340032
        private BinaryReader binaryReader;
        public int CountPartReadFile { get; private set; }
        public byte[] Buffer { get; set; }
        public int CountThreads { get; }
        public int BufferArraySize => Buffer.Length;
        private int currentPart, currentByte;
        private Action addProcess;
        private long partProgress;

        public ByteFile(string fullFileName)
        {
            binaryReader = new BinaryReader(File.OpenRead(fullFileName));
            partProgress = binaryReader.BaseStream.Length / 100;
            if (bufferSize < binaryReader.BaseStream.Length)
            {
                CountPartReadFile = (int)(binaryReader.BaseStream.Length / bufferSize);
                CountThreads = 4;
                ReadNewPartFile();
            }
            else
            {
                Buffer = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
                CountThreads = 1;
                binaryReader.Dispose();
            }


            currentPart = 0;
            currentByte = 0;


        }
        public ByteFile(string fullFileName, Action addProcess) : this(fullFileName)
        {
            this.addProcess = addProcess;
        }

        public bool isEnd()
        {
            if (currentPart == CountPartReadFile && currentByte == Buffer.Length)
            {
                binaryReader.Dispose();
                return true;
            }
            else
            {
                return false;
            }
        }
        public byte GetByte()
        {
            if ((Buffer.Length * currentPart + currentByte) % partProgress == 0)
                addProcess.Invoke();
            if (currentByte == Buffer.Length)
            {
                currentByte = 0;
                ReadNewPartFile();
            }
            return Buffer[currentByte++];
        }

        public bool GetBytesByIndex(long startBlock, long endBlock)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int index)
        {
            if ((Buffer.Length * currentPart + index) % partProgress == 0)
                addProcess.Invoke();
            return Buffer[index];
        }
        public void ReadNewPartFile()
        {
            if (currentPart < CountPartReadFile)
            {
                Buffer = binaryReader.ReadBytes(bufferSize);
                currentPart++;
            }
            else
            {
                Buffer = binaryReader.ReadBytes((int)(binaryReader.BaseStream.Length % bufferSize));
                binaryReader.Close();
            }
        }

        public void Dispose()
        {
            binaryReader.Close();
            Buffer = null;
            GC.Collect();

        }
    }
}
