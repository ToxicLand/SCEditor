using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCEditor.ScOld
{
    public class MovieClipModifierOriginal : ScObject
    {
        private ushort _id;
        private byte _tag;
        private ScFile _scFile;

        public MovieClipModifierOriginal(ScFile scfile)
        {
            this._scFile = scfile;
        }

        public override void Read(BinaryReader rd, string tag)
        {
            this._tag = Enumerable.Range(0, tag.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(tag.Substring(x, 2), 16))
                     .ToArray()[0];

            this._id = rd.ReadUInt16();
        }

        public ushort ID => this._id;

        public byte Tag => this._tag;

        public void SetID(ushort id)
        {
            this._id = id; ;
        }

        public void SetTag(byte tag)
        {
            this._tag = tag;
        }
    }
}
