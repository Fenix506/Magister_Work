using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Image.Formats
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ColorFloatPixel
    {
        public float b, g, r, a;
    }
    public class ColorFloatImage
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public readonly ColorFloatPixel[] rawdata;

        public ColorFloatImage(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            rawdata = new ColorFloatPixel[Width * Height];
        }

        public ColorFloatPixel this[int x, int y]
        {
            get
            {
#if DEBUG
                if (x < 0 || x >= Width || y < 0 || y >= Height)
                    throw new IndexOutOfRangeException(string.Format("Trying to access pixel ({0}, {1}) in {2}x{3} image", x, y, Width, Height));
#endif
                return rawdata[y * Width + x];
            }
            set
            {
#if DEBUG
                if (x < 0 || x >= Width || y < 0 || y >= Height)
                    throw new IndexOutOfRangeException(string.Format("Trying to access pixel ({0}, {1}) in {2}x{3} image", x, y, Width, Height));
#endif
                rawdata[y * Width + x] = value;
            }
        }

        public GrayscaleFloatImage ToGrayscaleFloatImageLightness()
        {
            var ResultImage = new GrayscaleFloatImage(Width, Height);
            for (int i = 0; i < rawdata.Length; i++)
            {
                ResultImage.rawdata[i] = (Max(rawdata[i].r, rawdata[i].g, rawdata[i].b) + Min(rawdata[i].r, rawdata[i].g, rawdata[i].b)) / 2.0f;

            }
            return ResultImage;
        }
        public GrayscaleFloatImage ToGrayscaleFloatImageAverage()
        {
            var ResultImage = new GrayscaleFloatImage(Width, Height);
            for (int i = 0; i < rawdata.Length; i++)
            {
                ResultImage.rawdata[i] = (rawdata[i].r + rawdata[i].g + rawdata[i].b) / 3.0f;
            }

            return ResultImage;
        }
        public GrayscaleFloatImage ToGrayscaleFloatImageLuminosity()
        {
            var ResultImage = new GrayscaleFloatImage(Width, Height);
            float a = 0.21f;
            float b = 0.72f;
            float c = 0.07f;
            for (int i = 0; i < rawdata.Length; i++)
            {
                ResultImage.rawdata[i] = a * rawdata[i].r + b * rawdata[i].g + c * rawdata[i].b;
            }
            return ResultImage;
        }

        private float Max(float r, float g, float b)
        {
            if (r > g)
            {
                if (r > b) return r;
                else return b;
            }
            else
            {
                if (g > b) return g;
                else return b;
            }
        }
        private float Min(float r, float g, float b)
        {
            if (r < g)
            {
                if (r < b) return r;
                else return b;
            }
            else
            {
                if (g < b) return g;
                else return b;
            }
        }
    }
}
