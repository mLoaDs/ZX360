#region Using Statements
using System;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Core
{
    public class Spectrum : Z80
    {
        public bool runAtFullSpeed = true;

        public static Hashtable patternMap = new Hashtable();

        public static Bitmap[] imageMap = new Bitmap[1 << 11]; // 7 bits for attr, 4 bits for pattern

        public int sleepHack = 2;
        public int refreshRate = 1;  // refresh every 'n' interrupts

        private int interruptCounter = 0;

        public long timeOfLastInterrupt = 0;

        public Spectrum()
            : base(3.5)
        {
        }

        #region Keyboard

        private static int b4 = 0x10;
        private static int b3 = 0x08;
        private static int b2 = 0x04;
        private static int b1 = 0x02;
        private static int b0 = 0x01;

        private int _B_SPC = 0xff;
        private int _H_ENT = 0xff;
        private int _Y_P = 0xff;
        private int _6_0 = 0xff;
        private int _1_5 = 0xff;
        private int _Q_T = 0xff;
        private int _A_G = 0xff;
        private int _CAPS_V = 0xff;

        public void K1(bool down)
        {
            if (down) _1_5 &= ~b0; else _1_5 |= b0;
        }
        public void K2(bool down)
        {
            if (down) _1_5 &= ~b1; else _1_5 |= b1;
        }
        public void K3(bool down)
        {
            if (down) _1_5 &= ~b2; else _1_5 |= b2;
        }
        public void K4(bool down)
        {
            if (down) _1_5 &= ~b3; else _1_5 |= b3;
        }
        public void K5(bool down)
        {
            if (down) _1_5 &= ~b4; else _1_5 |= b4;
        }
        public void K6(bool down)
        {
            if (down) _6_0 &= ~b4; else _6_0 |= b4;
        }
        public void K7(bool down)
        {
            if (down) _6_0 &= ~b3; else _6_0 |= b3;
        }
        public void K8(bool down)
        {
            if (down) _6_0 &= ~b2; else _6_0 |= b2;
        }
        public void K9(bool down)
        {
            if (down) _6_0 &= ~b1; else _6_0 |= b1;
        }
        public void K0(bool down)
        {
            if (down) _6_0 &= ~b0; else _6_0 |= b0;
        }


        public void KQ(bool down)
        {
            if (down) _Q_T &= ~b0; else _Q_T |= b0;
        }
        public void KW(bool down)
        {
            if (down) _Q_T &= ~b1; else _Q_T |= b1;
        }
        public void KE(bool down)
        {
            if (down) _Q_T &= ~b2; else _Q_T |= b2;
        }
        public void KR(bool down)
        {
            if (down) _Q_T &= ~b3; else _Q_T |= b3;
        }
        public void KT(bool down)
        {
            if (down) _Q_T &= ~b4; else _Q_T |= b4;
        }

        public void KY(bool down)
        {
            if (down) _Y_P &= ~b4; else _Y_P |= b4;
        }
        public void KU(bool down)
        {
            if (down) _Y_P &= ~b3; else _Y_P |= b3;
        }
        public void KI(bool down)
        {
            if (down) _Y_P &= ~b2; else _Y_P |= b2;
        }
        public void KO(bool down)
        {
            if (down) _Y_P &= ~b1; else _Y_P |= b1;
        }
        public void KP(bool down)
        {
            if (down) _Y_P &= ~b0; else _Y_P |= b0;
        }


        public void KA(bool down)
        {
            if (down) _A_G &= ~b0; else _A_G |= b0;
        }
        public void KS(bool down)
        {
            if (down) _A_G &= ~b1; else _A_G |= b1;
        }
        public void KD(bool down)
        {
            if (down) _A_G &= ~b2; else _A_G |= b2;
        }
        public void KF(bool down)
        {
            if (down) _A_G &= ~b3; else _A_G |= b3;
        }
        public void KG(bool down)
        {
            if (down) _A_G &= ~b4; else _A_G |= b4;
        }

        public void KH(bool down)
        {
            if (down) _H_ENT &= ~b4; else _H_ENT |= b4;
        }
        public void KJ(bool down)
        {
            if (down) _H_ENT &= ~b3; else _H_ENT |= b3;
        }
        public void KK(bool down)
        {
            if (down) _H_ENT &= ~b2; else _H_ENT |= b2;
        }
        public void KL(bool down)
        {
            if (down) _H_ENT &= ~b1; else _H_ENT |= b1;
        }
        public void KENT(bool down)
        {
            if (down) _H_ENT &= ~b0; else _H_ENT |= b0;
        }


        public void KCAPS(bool down)
        {
            if (down) _CAPS_V &= ~b0; else _CAPS_V |= b0;
        }
        public void KZ(bool down)
        {
            if (down) _CAPS_V &= ~b1; else _CAPS_V |= b1;
        }
        public void KX(bool down)
        {
            if (down) _CAPS_V &= ~b2; else _CAPS_V |= b2;
        }
        public void KC(bool down)
        {
            if (down) _CAPS_V &= ~b3; else _CAPS_V |= b3;
        }
        public void KV(bool down)
        {
            if (down) _CAPS_V &= ~b4; else _CAPS_V |= b4;
        }

        public void KB(bool down)
        {
            if (down) _B_SPC &= ~b4; else _B_SPC |= b4;
        }
        public void KN(bool down)
        {
            if (down) _B_SPC &= ~b3; else _B_SPC |= b3;
        }
        public void KM(bool down)
        {
            if (down) _B_SPC &= ~b2; else _B_SPC |= b2;
        }
        public void KSYMB(bool down)
        {
            if (down) _B_SPC &= ~b1; else _B_SPC |= b1;
        }
        public void KSPC(bool down)
        {
            if (down) _B_SPC &= ~b0; else _B_SPC |= b0;
        }




        public void Fire(bool down)
        {
            if (down) _6_0 &= ~b0; else _6_0 |= b0;
        }

        public void Up(bool down)
        {
            if (down) _6_0 &= ~b3; else _6_0 |= b3;
        }

        public void Down(bool down)
        {
            if (down) _6_0 &= ~b4; else _6_0 |= b4;
        }

        public void Left(bool down)
        {
            if (down) _1_5 &= ~b4; else _1_5 |= b4;
        }

        public void Right(bool down)
        {
            if (down) _6_0 &= ~b2; else _6_0 |= b2;
        }

        #endregion

        #region Harware Interface

        public override int inb(int port)
        {
            int res = 0xff;

            if ((port & 0x0001) == 0)
            {
                if ((port & 0x8000) == 0) 
                    { res &= _B_SPC; }
                if ((port & 0x4000) == 0) 
                    { res &= _H_ENT; }
                if ((port & 0x2000) == 0) 
                    { res &= _Y_P; }
                if ((port & 0x1000) == 0) 
                    { res &= _6_0; }
                if ((port & 0x0800) == 0) 
                    { res &= _1_5; }
                if ((port & 0x0400) == 0) 
                    { res &= _Q_T; }
                if ((port & 0x0200) == 0) 
                    { res &= _A_G; }
                if ((port & 0x0100) == 0) 
                    { res &= _CAPS_V; }
            }

            return (res);
        }

        public override void outb(int port, int outByte, int tstates)
        {
            if ((port & 0x0001) == 0)
            {
                newBorder = (outByte & 0x07);

                sound = (outByte & 0x10);                
            }
        }

        public int Sound
        {
            get { return this.sound; }
        }

        /** Byte access */
        public override void pokeb(int addr, int newByte)
        {
            if (addr >= (22528 + 768))
            {
                mem[addr] = newByte;
                return;
            }

            if (addr < 16384)
            {
                return;
            }

            if (mem[addr] != newByte)
            {
                plot(addr, newByte);
                mem[addr] = newByte;
            }
        }

        // Word access
        public override void pokew(int addr, int word)
        {
            int[] _mem = mem;

            if (addr >= (22528 + 768))
            {
                _mem[addr] = word & 0xff;
                if (++addr != 65536)
                {
                    _mem[addr] = word >> 8;
                }
                return;
            }

            if (addr < 16384)
            {
                return;
            }

            int newByte0 = word & 0xff;
            if (_mem[addr] != newByte0)
            {
                plot(addr, newByte0);
                _mem[addr] = newByte0;
            }

            int newByte1 = word >> 8;
            if (++addr != (22528 + 768))
            {
                if (_mem[addr] != newByte1)
                {
                    plot(addr, newByte1);
                    _mem[addr] = newByte1;
                }
            }
            else
            {
                _mem[addr] = newByte1;
            }
        }

        public override int interrupt()
        {
            interruptCounter++;

            // Characters flash every 1/2 a second
            if ((interruptCounter % 25) == 0)
            {
                refreshFlashChars();
            }

            // Refresh every interrupt by default
            if ((interruptCounter % refreshRate) == 0)
            {
                refreshWholeScreen();
                screenPaint();
            }

            return base.interrupt();
        }

        public override void reset()
        {
            base.reset();

            outb(254, 0xff, 0); // White border on startup
        }
        #endregion

        public const int pixelScale = 1;    // scales pixels in main screen, not border

        public const int nPixelsWide = 256;
        public const int nPixelsHigh = 192;
        public const int nCharsWide = 32;
        public const int nCharsHigh = 24;

        private const int sat = 238;

        private static Color[] brightColors = {
            new Color(   0,   0,   0 ),  new Color(   0,   0, sat ),
            new Color( sat,   0,   0 ),  new Color( sat,   0, sat ),
            new Color(   0, sat,   0 ),  new Color(   0, sat, sat ),
            new Color( sat, sat,   0 ),  new Color( sat, sat, sat ),
            Color.Black,                 Color.Blue,
            Color.Red,                   Color.Magenta,
            Color.Green,                 Color.Cyan, 
            Color.Yellow,                Color.White
        };

        private const int firstAttr = (nPixelsHigh * nCharsWide);
        private const int lastAttr = firstAttr + (nCharsHigh * nCharsWide);

        /** first screen line in linked list to be redrawn */
        private int first = -1;
        /** first attribute in linked list to be redrawn */
        private int FIRST = -1;
        private int[] last = new int[(nPixelsHigh + nCharsHigh) * nCharsWide];
        private int[] next = new int[(nPixelsHigh + nCharsHigh) * nCharsWide];

        public int newBorder = 7;  // White border on startup
        public int sound = 0;

        private bool flashInvert = false;

        private void refreshFlashChars()
        {
            flashInvert = !flashInvert;

            for (int i = firstAttr; i < lastAttr; i++)
            {
                int attr = mem[i + 16384];

                if ((attr & 0x80) != 0)
                {
                    last[i] = (~attr) & 0xff;

                    // Only add to update list if not already marked 
                    if (next[i] == -1)
                    {
                        next[i] = FIRST;
                        FIRST = i;
                    }
                }
            }
        }

        public void refreshWholeScreen()
        {
            for (int i = 0; i < firstAttr; i++)
            {
                next[i] = i - 1;
                last[i] = (~mem[i + 16384]) & 0xff;
            }

            for (int i = firstAttr; i < lastAttr; i++)
            {
                next[i] = -1;
                last[i] = mem[i + 16384];
            }

            first = firstAttr - 1;
            FIRST = -1;
        }

        private void plot(int addr, int newByte)
        {
            int offset = addr - 16384;

            if (next[offset] == -1)
            {
                if (offset < firstAttr)
                {
                    next[offset] = first;
                    first = offset;
                }
                else
                {
                    next[offset] = FIRST;
                    FIRST = offset;
                }
            }
        }

        public uint[] ScreenBuffer
        {
            get { return this.screenBuffer; }
        }

        public static Bitmap getImage(int attr, int pattern)
        {
            try
            {
                return tryGetImage(attr, pattern);
            }
            catch (OutOfMemoryException e)
            {
                imageMap = null;
                patternMap = null;

                GC.Collect();

                patternMap = new Hashtable();
                imageMap = new Bitmap[1 << 11];

                return tryGetImage(attr, pattern);
            }
        }

        private static Bitmap tryGetImage(int attr, int pattern)
        {
            int bright = ((attr >> 3) & 0x08);
            int ink = ((attr) & 0x07) | bright;
            int pap = ((attr >> 3) & 0x07) | bright;
            int hashValue = 0;

            for (int i = 0; i < 4; i++)
            {
                int col = ((pattern & (1 << i)) == 0) ? pap : ink;
                hashValue |= (col << (i << 2));
            }

            Bitmap bitmap = (Bitmap)patternMap[hashValue];
            if (bitmap == null)
            {
                Color[] colors = brightColors;

                bitmap = new Bitmap();

                for (int i = 0; i < 4; i++)
                {
                    int col = ((pattern & (1 << i)) == 0) ? pap : ink;

                    bitmap.SetPixel((3 - i), colors[col]);
                }

                patternMap.Add(hashValue, bitmap);
            }

            return bitmap;
        }

        private uint[] screenBuffer = new uint[256 * 192];

        public void screenPaint()
        {
            int addr = FIRST;

            // Update attribute affected pixels
            while (addr >= 0)
            {
                int oldAttr = last[addr];
                int newAttr = mem[addr + 16384];
                last[addr] = newAttr;

                bool inkChange = ((oldAttr & 0x47) != (newAttr & 0x47));
                bool papChange = ((oldAttr & 0x78) != (newAttr & 0x78));
                bool flashChange = ((oldAttr & 0x80) != (newAttr & 0x80));

                if (inkChange || papChange || flashChange)
                {
                    bool allChange = ((inkChange && papChange) || flashChange);
                    int scrAddr = ((addr & 0x300) << 3) | (addr & 0xff);

                    for (int i = 8; i != 0; i--)
                    {
                        if (allChange)
                        {
                            last[scrAddr] = ((~mem[scrAddr + 16384]) & 0xff);
                        }
                        else
                        {
                            int oldPixels = last[scrAddr];
                            int newPixels = mem[scrAddr + 16384];
                            int changes = oldPixels ^ newPixels;

                            if (inkChange)
                            {
                                changes |= newPixels;
                            }
                            else
                            {
                                changes |= ((~newPixels) & 0xff);
                            }
                            if (changes == 0)
                            {
                                scrAddr += 256;
                                continue;
                            }
                            last[scrAddr] = changes ^ newPixels;
                        }

                        if (next[scrAddr] == -1)
                        {
                            next[scrAddr] = first;
                            first = scrAddr;
                        }

                        scrAddr += 256;
                    }
                }

                int newAddr = next[addr];
                next[addr] = -1;
                addr = newAddr;
            }
            FIRST = -1;

            // Only update screen if necessary
            if (first < 0)
            {
                return;
            }

            // Update affected pixels
            addr = first;
            while (addr >= 0)
            {
                int oldPixels = last[addr];
                int newPixels = mem[addr + 16384];
                int changes = oldPixels ^ newPixels;
                last[addr] = newPixels;

                int x = ((addr & 0x1f) << 3);
                int y = (((int)(addr & 0x00e0)) >> 2) +
                    (((int)(addr & 0x0700)) >> 8) +
                    (((int)(addr & 0x1800)) >> 5);
                int X = (x * pixelScale);
                int Y = (y * pixelScale);

                int attr = mem[22528 + (addr & 0x1f) + ((y >> 3) * nCharsWide)];

                // Swap colors around if doing flash
                if (flashInvert && ((attr & 0x80) != 0))
                {
                    newPixels = (~newPixels & 0xff);
                }

                // Redraw left nibble if necessary
                if ((changes & 0xf0) != 0)
                {
                    int newPixels1 = (newPixels & 0xf0) >> 4;
                    int imageMapEntry1 = (((attr & 0x7f) << 4) | newPixels1);
                    Bitmap image1 = imageMap[imageMapEntry1];
                    if (image1 == null)
                    {
                        image1 = getImage(attr, newPixels1);
                        imageMap[imageMapEntry1] = image1;
                    }

                    int location = X + (Y * 256);

                    for (int i = 0; i < 4; i++)
                    {
                        screenBuffer[location + i] = image1.Data[i];
                    }
                }

                // Redraw right nibble if necessary
                if ((changes & 0x0f) != 0)
                {
                    int newPixels2 = (newPixels & 0x0f);
                    int imageMapEntry2 = (((attr & 0x7f) << 4) | newPixels2);
                    Bitmap image2 = imageMap[imageMapEntry2];
                    if (image2 == null)
                    {
                        image2 = getImage(attr, newPixels2);
                        imageMap[imageMapEntry2] = image2;
                    }

                    int location = X + (Y * 256);

                    for (int i = 0; i < 4; i++)
                    {
                        screenBuffer[location + i + 4] = image2.Data[i];
                    }
                }

                int newAddr = next[addr];
                next[addr] = -1;
                addr = newAddr;
            }
            first = -1;
        }

        #region ROM Loading
        public int bytesReadSoFar = 0;
        public int bytesToReadTotal = 0;

        private void loadROM(BinaryReader _is)
        {
            int bytesRead = readBytes(_is, mem, 0, 16384);
        }

        private void loadSNA(BinaryReader _is)
        {
            int[] header = new int[27];

            readBytes(_is, header, 0, 27);
            readBytes(_is, mem, 16384, 49152);

            I(header[0]);

            HL(header[1] | (header[2] << 8));
            DE(header[3] | (header[4] << 8));
            BC(header[5] | (header[6] << 8));
            AF(header[7] | (header[8] << 8));

            exx();
            ex_af_af();

            HL(header[9] | (header[10] << 8));
            DE(header[11] | (header[12] << 8));
            BC(header[13] | (header[14] << 8));

            IY(header[15] | (header[16] << 8));
            IX(header[17] | (header[18] << 8));

            if ((header[19] & 0x04) != 0)
            {
                IFF2(true);
            }
            else
            {
                IFF2(false);
            }

            R(header[20]);

            AF(header[21] | (header[22] << 8));
            SP(header[23] | (header[24] << 8));

            switch (header[25])
            {
                case 0:
                    IM(IM0);
                    break;
                case 1:
                    IM(IM1);
                    break;
                default:
                    IM(IM2);
                    break;
            }

            outb(254, header[26], 0); // border

            /* Emulate RETN to start */
            IFF1(IFF2());
            REFRESH(2);
            poppc();
        }

        public void LoadROM(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                using (BinaryReader r = new BinaryReader(fs))
                {
                    this.loadROM(r);
                }
            }
        }

        public void LoadZ80(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                using (BinaryReader r = new BinaryReader(fs))
                {
                    this.loadZ80(r, (int)fs.Length);
                }
            }
        }

        public void LoadSNA(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                using (BinaryReader r = new BinaryReader(fs))
                {
                    this.loadSNA(r);
                }
            }
        }

        private void loadZ80(BinaryReader _is, int bytesLeft)
        {
            int[] header = new int[30];
            bool compressed = false;

            bytesLeft -= readBytes(_is, header, 0, 30);

            A(header[0]);
            F(header[1]);

            C(header[2]);
            B(header[3]);
            L(header[4]);
            H(header[5]);

            PC(header[6] | (header[7] << 8));
            SP(header[8] | (header[9] << 8));

            I(header[10]);
            R(header[11]);

            int tbyte = header[12];
            if (tbyte == 255)
            {
                tbyte = 1;
            }

            outb(254, ((tbyte >> 1) & 0x07), 0); // border

            if ((tbyte & 0x01) != 0)
            {
                R(R() | 0x80);
            }
            compressed = ((tbyte & 0x20) != 0);

            E(header[13]);
            D(header[14]);

            ex_af_af();
            exx();

            C(header[15]);
            B(header[16]);
            E(header[17]);
            D(header[18]);
            L(header[19]);
            H(header[20]);

            A(header[21]);
            F(header[22]);

            ex_af_af();
            exx();

            IY(header[23] | (header[24] << 8));
            IX(header[25] | (header[26] << 8));

            IFF1(header[27] != 0);
            IFF2(header[28] != 0);

            switch (header[29] & 0x03)
            {
                case 0:
                    IM(IM0);
                    break;
                case 1:
                    IM(IM1);
                    break;
                default:
                    IM(IM2);
                    break;
            }

            if (PC() == 0)
            {
                //loadZ80_extended(_is, bytesLeft);
                return;
            }

            /* Old format Z80 snapshot */

            if (compressed)
            {
                int[] data = new int[bytesLeft];
                int addr = 16384;

                int size = readBytes(_is, data, 0, bytesLeft);
                int i = 0;

                while ((addr < 65536) && (i < size))
                {
                    tbyte = data[i++];
                    if (tbyte != 0xed)
                    {
                        pokeb(addr, tbyte);
                        addr++;
                    }
                    else
                    {
                        tbyte = data[i++];
                        if (tbyte != 0xed)
                        {
                            pokeb(addr, 0xed);
                            i--;
                            addr++;
                        }
                        else
                        {
                            int count;
                            count = data[i++];
                            tbyte = data[i++];
                            while ((count--) != 0)
                            {
                                pokeb(addr, tbyte);
                                addr++;
                            }
                        }
                    }
                }
            }
            else
            {
                readBytes(_is, mem, 16384, 49152);
            }
        }

        private int readBytes(BinaryReader _is, int[] a, int off, int n)
        {
            try
            {

                byte[] buff = new byte[n];
                int toRead = n;
                while (toRead > 0)
                {
                    int nRead = _is.Read(buff, n - toRead, toRead);
                    toRead -= nRead;
                }

                for (int i = 0; i < n; i++)
                {
                    a[i + off] = (buff[i] + 256) & 0xff;
                }

                return n;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private int loadZ80_page(int[] data, int i)
        {
            int blocklen;
            int page;

            blocklen = data[i++];
            blocklen |= (data[i++]) << 8;
            page = data[i++];

            int addr;
            switch (page)
            {
                case 4:
                    addr = 32768;
                    break;
                case 5:
                    addr = 49152;
                    break;
                case 8:
                    addr = 16384;
                    break;
                default:
                    throw new Exception("Z80 (page): out of range " + page);
            }

            int k = 0;
            while (k < blocklen)
            {
                int tbyte = data[i++]; k++;
                if (tbyte != 0xed)
                {
                    pokeb(addr, ~tbyte);
                    pokeb(addr, tbyte);
                    addr++;
                }
                else
                {
                    tbyte = data[i++]; k++;
                    if (tbyte != 0xed)
                    {
                        pokeb(addr, 0);
                        pokeb(addr, 0xed);
                        addr++;
                        i--; k--;
                    }
                    else
                    {
                        int count;
                        count = data[i++]; k++;
                        tbyte = data[i++]; k++;
                        while (count-- > 0)
                        {
                            pokeb(addr, ~tbyte);
                            pokeb(addr, tbyte);
                            addr++;
                        }
                    }
                }
            }

            if ((addr & 16383) != 0)
            {
                throw new Exception("Z80 (page): overrun");
            }

            return i;
        }
        #endregion

    }
}