using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Graphics;

namespace Core
{
    public class Bitmap
    {
        private uint[] m_pixels = new uint[4];

        public void SetPixel(int index, Color color)
        {
            this.m_pixels[index] = color.PackedValue;
        }

        public uint[] Data
        {
            get { return this.m_pixels; }
        }

    }
}
