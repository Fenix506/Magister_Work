using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler
{
    public class ByteFile
    {

        private const int bufferSize = 104857600;
        private byte[] buffer;
        private BinaryReader binaryReader;
        private int countPartReadFile;
        private int currentPart, currentByte;
        private Action addProcess, finish;
        private long partProgress;

        public ByteFile(string fullFileName)
        {
            binaryReader = new BinaryReader(File.OpenRead(fullFileName));
            if (bufferSize > binaryReader.BaseStream.Length)
            {
                countPartReadFile = (int)(binaryReader.BaseStream.Length / bufferSize);
            }
            else
            {
                buffer = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
            }
            partProgress = binaryReader.BaseStream.Length / 100;
            ReadNewPartFile();
            currentPart = 0;
            currentByte = 0;
        }
        public ByteFile(string fullFileName, Action addProcess, Action finish) : this(fullFileName)
        {
            this.addProcess = addProcess;
            this.finish = finish;
        }

        public bool isEnd()
        {
            if (currentPart == countPartReadFile && currentByte == buffer.Length)
            {
                binaryReader.Dispose();
                finish.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }
        public byte getByte()
        {
            if ((buffer.Length * currentPart + currentByte + 1) % partProgress == 0)
                addProcess.Invoke();
            if (currentByte == buffer.Length)
            {
                currentByte = 0;
                ReadNewPartFile();
            }
            return buffer[currentByte++];
        }
        private void ReadNewPartFile()
        {
            if (currentPart < countPartReadFile)
            {
                buffer = binaryReader.ReadBytes(bufferSize);
                currentPart++;
            }
            else
            {
                buffer = binaryReader.ReadBytes((int)(binaryReader.BaseStream.Length % bufferSize));

            }
        }
    }
}
