using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCEditor.ScOld
{
    public struct ColorTransform
    {
        public static readonly ColorTransform Default = new ColorTransform(0xFF, 0xFF, 0xFF, 0xFF, 0, 0, 0);

        public byte r; // 0
        public byte g; // 1
        public byte b; // 2
        public byte alpha; // 3
        public byte mulr; // 4
        public byte mulg; // 5
        public byte mulb; // 6

        public ColorTransform(byte r, byte g, byte b, byte alpha, byte mulr, byte mulg, byte mulb)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.alpha = alpha;
            this.mulr = mulr;
            this.mulg = mulg;
            this.mulb = mulb;
        }

        public void SetMulColor(float r, float g, float b)
        {
            this.mulr = (byte)(Math.Clamp(r, 0, 1) * 255);
            this.mulg = (byte)(Math.Clamp(g, 0, 1) * 255);
            this.mulb = (byte)(Math.Clamp(b, 0, 1) * 255);
        }

        public void SetAlpha(float a)
        {
            this.alpha = (byte)(Math.Clamp(a, 0, 1) * 255);
        }

        public readonly void Multiply(in ColorTransform multiply, out ColorTransform output)
        {
            output.r = (byte)(this.r * multiply.r / 255);
            output.g = (byte)(this.g * multiply.g / 255);
            output.b = (byte)(this.b * multiply.b / 255);
            output.alpha = (byte)(this.alpha * multiply.alpha / 255);

            int mulr = this.mulr + multiply.mulr;
            int mulg = this.mulg + multiply.mulg;
            int mulb = this.mulb + multiply.mulb;

            if (mulr + mulg + mulb != 0)
            {
                if (mulr > 255)
                    mulr = 255;
                if (mulg > 255)
                    mulg = 255;
                if (mulb > 255)
                    mulb = 255;
            }

            output.mulr = (byte)mulr;
            output.mulg = (byte)mulg;
            output.mulb = (byte)mulb;
        }
    }
}
