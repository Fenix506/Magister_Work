using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Image.Formats
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ColorBytePixel
    {
        public byte b, g, r, a;
    }

    public class ColorByteImage
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public readonly ColorBytePixel[] rawdata;

        public ColorByteImage(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            rawdata = new ColorBytePixel[Width * Height];
        }

        public ColorBytePixel this[int x, int y]
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

        public void DrawRectangle(int startX, int startY, int endX, int endY, ColorBytePixel color)
        {
            for (int x = 0; x < Math.Abs(endX - startX); x++)
            {
                if (startX + x > 0 && startX + x < Width && startY >= 0 && startY < Height) this[startX + x, startY] = color;
                if (endX - x > 0 && endX - x < Width && endY >= 0 && endY < Height) this[endX - x, endY] = color;
            }
            for (int y = 0; y < Math.Abs(endY - startY); y++)
            {
                if (startY - y > 0 && startY - y < Height && startX >= 0 && startX < Width) this[startX, startY - y] = color;
                if (endY + y > 0 && endY + y < Height && endX >= 0 && endX < Width) this[endX, endY + y] = color;
            }
        }
    }
}
