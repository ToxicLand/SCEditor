using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SCEditor.Sc.Loaders
{
    // Loader to load .sc version 7.x.x.
    internal class ScLoader7 : ScLoader
    {
        public override void Load(ref ScFile file, Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var shapeCount = reader.ReadUInt16(); // a1 + 8
                var movieClipCount = reader.ReadUInt16(); // a1 + 12
                var textureCount = reader.ReadUInt16(); // a1 + 16
                var textFieldCount = reader.ReadUInt16(); // a1 + 24
                var matrixCount = reader.ReadUInt16(); // a1 + 28
                var colorTransformCount = reader.ReadUInt16(); // a1 + 32

                // 5 useless bytes, not even used by Supercell
                reader.ReadByte(); // 1 octet
                reader.ReadUInt16(); // 2 octets
                reader.ReadUInt16(); // 2 octets

                var exportCount = reader.ReadUInt16();
                var exports = new List<Export>(exportCount);

                // Reading the exports Ids.
                for (int i = 0; i < exportCount; i++)
                {
                    var export = new Export(file);
                    var exportId = reader.ReadUInt16();
                    export._id = exportId;

                    exports.Add(export);
                }

                // Reading the export names.
                for (int i = 0; i < exportCount; i++)
                {
                    var export = exports[i];
                    var exportNameLen = reader.ReadByte();
                    var exportName = Encoding.UTF8.GetString(reader.ReadBytes(exportNameLen));

                    export._name = exportName;
                }

                do
                {
                    var typeId = reader.ReadByte();
                    var length = reader.ReadInt32();
                    var bytes = reader.ReadBytes(length);

                    if (typeId == 0)
                        break;
                }
                while (true);
            }
        }
    }
}
