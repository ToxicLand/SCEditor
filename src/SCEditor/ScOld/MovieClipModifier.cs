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
        private string _tag;
        private ScFile _scFile;

        public MovieClipModifier(ScFile scfile)
        {
            this._scFile = scfile;
        }

        public override void Read(BinaryReader rd, string tag)
        {
            _tag = tag;
            Id = rd.ReadUInt16();
        }

        public string tag => _tag;
    }
}
