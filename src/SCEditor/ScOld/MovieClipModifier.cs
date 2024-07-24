using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCEditor.ScOld
{
    public class MovieClipModifier : ScObject
    {
        private byte _tag;
        private ScFile _scFile;

        public MovieClipModifier(ScFile scfile)
        {
            this._scFile = scfile;
        }

        public override void Read(ScFile swf, BinaryReader rd, byte tag)
        {
            _tag = tag;
            Id = rd.ReadUInt16();
        }

        public byte Tag => _tag;
    }
}
