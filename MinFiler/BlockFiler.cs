﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler
{
    public class BlockFiler
    {
        private string fullFileName;
        private double blockEntropy;
        public LinkedList<Block> Blocks { get; set; }
        public double FileEntropy { get; set; }

        public event Action AddProgress;
        public event Action Finish;
        public BlockFiler(string fullFileName, double blockEntropy = 4)
        {
            this.fullFileName = fullFileName;
            this.blockEntropy = blockEntropy;
            Blocks = new LinkedList<Block>();

        }
        public void Bloking()
        {
            uint[] fullCountBytes = new uint[256];
            byte[] blockCountBytes = new byte[256];

            byte readByte;
            Block currentBlock = new Block();
            double currentEntropy, previousEntropy = 0;
            using (var binaryReader = new BinaryReader(File.OpenRead(fullFileName)))
            {
                long part = binaryReader.BaseStream.Length / 100;
                for (long i = 0; i < binaryReader.BaseStream.Length; i++)
                {
                    readByte = binaryReader.ReadByte();
                    fullCountBytes[readByte]++;
                    blockCountBytes[readByte]++;
                    if (i % part == 0)
                    {
                        AddProgress?.Invoke();
                    }
                    currentEntropy = Entropy(blockCountBytes);
                    if (currentEntropy < blockEntropy)
                    {
                        currentBlock.Data.Add(readByte);
                    }
                    else
                    {
                        currentBlock.Entropy = previousEntropy;
                        Blocks.AddLast(currentBlock);
                        currentBlock = new Block();
                        currentBlock.Data.Add(readByte);
                        blockCountBytes = new byte[256];
                        blockCountBytes[readByte]++;
                    }
                    previousEntropy = currentEntropy;

                }
                FileEntropy = Entropy(fullCountBytes);
                Finish?.Invoke();
            }

        }
        public void Bloking_v2()
        {
            uint[] fullCountBytes = new uint[256];
            byte[] blockCountBytes = new byte[256];
            var file = new ByteFile(fullFileName, AddProgress, Finish);

            byte readByte;
            Block currentBlock = new Block();
            double currentEntropy, previousEntropy = 0;
            while (!file.isEnd())
            {
                readByte = file.getByte();
                fullCountBytes[readByte]++;
                blockCountBytes[readByte]++;

                currentEntropy = Entropy(blockCountBytes);
                if (currentEntropy < blockEntropy)
                {
                    currentBlock.Data.Add(readByte);
                }
                else
                {
                    currentBlock.Entropy = previousEntropy;
                    Blocks.AddLast(currentBlock);
                    currentBlock = new Block();
                    currentBlock.Data.Add(readByte);
                    blockCountBytes = new byte[256];
                    blockCountBytes[readByte]++;
                }
                previousEntropy = currentEntropy;

            }
            FileEntropy = Entropy(fullCountBytes);
        }
        public async Task BlokingAsync()
        {
            await Task.Run(() => Bloking());

        }
        public async Task Bloking_Ver2Async()
        {
            await Task.Run(() => Bloking_v2());
        }
        private double Entropy(byte[] arr)
        {
            uint length = 0;
            double entropy = 0;
            for (int i = 0; i < 256; i++)
            {
                checked
                {
                    length += arr[i];
                }
            }
            for (int i = 0; i < 256; i++)
            {
                if (arr[i] > 0)
                {
                    double pi = (double)arr[i] / length;

                    checked { entropy += pi * Math.Log(pi, 2); }
                }
            }
            return -entropy;
        }
        private double Entropy(uint[] arr)
        {
            uint length = 0;
            double entropy = 0;
            for (int i = 0; i < 256; i++)
            {
                checked
                {
                    length += arr[i];
                }
            }
            for (int i = 0; i < 256; i++)
            {
                if (arr[i] > 0)
                {
                    double pi = (double)arr[i] / length;

                    checked { entropy += pi * Math.Log(pi, 2); }
                }
            }
            return -entropy;
        }

    }
}