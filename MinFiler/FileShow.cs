using Image.Formats;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler
{
    public class FileShow
    {
        private string FilePath;
        private string ImagePath;
        private ulong[] counts;
        public FileShow(string filePath)
        {
            counts = new ulong[256];
            this.FilePath = filePath;
            this.ImagePath = Path.Combine(Directory.CreateDirectory("Images").FullName, DateTime.Now.ToString("HHmmss"));
        }
        public void Create()
        {
            int bufferSize = 104857600; //100Mb
            var file = new FileInfo(FilePath);
            int countread = (int)(file.Length / bufferSize);
            using (var binaryReader = new BinaryReader(File.OpenRead(FilePath)))
            {

                for (int i = 0; i < countread; i++)
                {
                    AddPixel(binaryReader.ReadBytes(bufferSize));
                }
                if (file.Length % bufferSize != 0)
                {
                    AddPixel(binaryReader.ReadBytes((int)(file.Length % bufferSize)));
                }

            }
        }
        private void AddPixel(byte[] arr)
        {
            ulong[] part = new ulong[256];
            ulong bytes = 0;
            double prev = 0;
            int unic = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                var entr = PartEntropy(part);

                if (entr <= 3 && i < 10220)
                {
                    if (part[arr[i]] == 0)unic++;
                    part[arr[i]]++;

                    bytes++;
                }
                else
                {
                    if (i < 2500)
                    {
                        Console.WriteLine($"For {bytes-1} bytes {unic} unic Entropy = {prev}");
                    }

                    part = new ulong[256];
                    unic = 1; ;
                    part[arr[i]]++;
                    bytes = 1;
                }
                counts[arr[i]]++;
                prev = entr;
            }
            Console.WriteLine($"For block {arr.Length} bytes Entropy = {PartEntropy(counts)}");
        }
        public GrayscaleByteImage getImage()
        {
            ulong maxvalue = counts.Max();
            var img = new GrayscaleByteImage(256, (int)(maxvalue / 255 + 5));
            for (int i = 0; i < 256; i++)
            {
                var tmp = counts[i];
                int countPixels = (int)(tmp / 255);
                for (int j = 0; j < countPixels; j++)
                {
                    img[i, j] = 255;
                }
                if (tmp % 255 != 0) img[i, countPixels + 1] = (byte)(tmp % 255);
            }
            return img;
        }
        public double Entropy()
        {
            double entr = 0;
            ulong length = 0;
            for (int i = 0; i < 256; i++)
            {
                checked
                {
                    length += counts[i];
                }
            }
            for (int i = 0; i < 256; i++)
            {
                double pi = (double)counts[i] / length;

                checked { entr += pi * Math.Log(pi, 2); }

            }
            return -entr;
        }
        public double PartEntropy(ulong[] arr)
        {
            double entr = 0;
            ulong length = 0;
            int numbers = 0;
            for (int i = 0; i < 256; i++)
            {
                if (arr[i] != 0)
                {

                    checked
                    {
                        length += arr[i];
                    }
                }
            }
            for (int i = 0; i < 256; i++)
            {
                if (arr[i] != 0)
                {
                    double pi = (double)arr[i] / length;

                    checked { entr += pi * Math.Log(pi, 2); }
                }
            }
            return -entr;
        }
    }
}
