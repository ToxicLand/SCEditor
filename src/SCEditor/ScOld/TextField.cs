using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCEditor.ScOld
{
    class TextField : ScObject
    {
        private string _fontName;
        private ushort _unk16;
        private ushort _unk18;
        private ushort _unk20;
        private ushort _unk22;
        private int _unk24;
        private int _unk28;
        private string _unk32;
        private byte _flag;
        private byte _unk42;
        private byte _unk43;
        private ushort _unk44;
        private ushort _unk46;
        private ScFile _scFile;

        public string fontName => _fontName;

        public TextField(ScFile scFile)
        {
            _scFile = scFile;
        }

        public override int GetDataType()
        {
            return 5;
        }

        public override string GetDataTypeName()
        {
            return "TextFields";
        }

        public override void Read(BinaryReader br, string packetid)
        {
            Id = br.ReadUInt16();

            byte stringLength = br.ReadByte();
            if (stringLength < 255)
            {
                _fontName = Encoding.ASCII.GetString(br.ReadBytes(stringLength));
            }

            if (!string.IsNullOrEmpty(_fontName))
            {
                if (_scFile.getFontNames().FindIndex(n => n == _fontName) == -1)
                    _scFile.addFontName(_fontName);
            }

            _unk24 = br.ReadInt32();

            if (br.ReadBoolean())
                _flag |= 4;
            if (br.ReadBoolean())
                _flag |= 8;
            if (br.ReadBoolean())
                _flag |= 16;

            br.ReadBoolean();
            _unk42 = br.ReadByte();
            _unk43 = br.ReadByte();
            _unk16 = br.ReadUInt16();
            _unk18 = br.ReadUInt16();
            _unk20 = br.ReadUInt16();
            _unk22 = br.ReadUInt16();

            if (br.ReadBoolean())
                _flag |= 2;

            byte stringLength2 = br.ReadByte();
            if (stringLength2 < 255)
            {
                _unk32 = Encoding.ASCII.GetString(br.ReadBytes(stringLength2));
            }

            if (packetid == "07")
                return;

            if (br.ReadBoolean())
                _flag |= 1;

            switch (packetid)
            {
                case "14":
                    _flag |= 32;
                    break;
                case "15":
                    _flag |= 32;
                    _unk28 = br.ReadInt32();;
                    break;
                case "19":
                    _unk28 = br.ReadInt32();;
                    break;

                case "21":
                case "2B":
                case "2C":
                    _unk28 = br.ReadInt32();;
                    _unk44 = br.ReadUInt16();
                    br.ReadUInt16();
                    _flag |= 32;

                    if (packetid == "2B" || packetid == "2C")
                    {
                        ushort tmp = br.ReadUInt16();
                        _unk46 = (ushort)(((uint)(0x7FFF * tmp + ((-40656255836343L * tmp) >> 32)) >> 31)
                                                 + ((uint)(0x7FFF * tmp + ((-40656255836343L * tmp) >> 32)) >> 8));
                    }

                    if (packetid == "2C" && br.ReadBoolean())
                        _flag |= 64;

                    break;
            }
        }

        public override Bitmap Render(RenderingOptions options)
        {
            return null;
        }
    }
}
