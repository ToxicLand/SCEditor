using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.IO;
using System.Windows.Forms;
using SCEditor.Compression;

namespace SCEditor.ScOld
{
    public class ScFile
    {

        #region Constructors

        public ScFile(string infoFile, string textureFile)
        {
            _textures = new List<ScObject>();
            _shapes = new List<ScObject>();
            _exports = new List<ScObject>();
            _movieClips = new List<ScObject>();
            _movieClipsModifier = new List<ScObject>();
            _pendingChanges = new List<ScObject>();
            _pendingMatrixs = new List<Matrix>();
            _matrixs = new List<Matrix>();
            _colors = new List<Tuple<Color, byte, Color>>();
            _infoFile = infoFile;
            _textureFile = textureFile;
        }

        #endregion

        #region Fields & Properties

        private ushort _exportCount;
        private ushort _shapeCount;
        private ushort _movieClipCount;
        private ushort _textureCount;
        private int _textFieldCount; 
        private int _matrixCount;
        private readonly List<ScObject> _textures;
        private readonly List<ScObject> _shapes;
        private readonly List<ScObject> _exports;
        private readonly List<Matrix> _matrixs;
        private readonly List<Tuple<Color, byte, Color>> _colors;
        private readonly List<ScObject> _movieClips;
        private readonly List<ScObject> _pendingChanges;
        private readonly List<ScObject> _movieClipsModifier;
        private readonly List<Matrix> _pendingMatrixs;

        private readonly string _infoFile;
        private readonly string _textureFile;
        private long _eofOffset;
        private long _eofMatrixOffset;
        private long _eofTexOffset;
        private long _exportStartOffset;
        private long _sofTagsOffset;

        #endregion

        public void AddChange(ScObject change)
        {
            if (_pendingChanges.IndexOf(change) == -1)
                _pendingChanges.Add(change);
        }

        public void AddExport(Export export)
        {
            _exports.Add(export);
        }

        public void AddShape(Shape shape)
        {
            _shapes.Add(shape);
        }

        public void AddTexture(Texture texture)
        {
            _textures.Add(texture);
        }

        public void AddMovieClip(MovieClip movieClip)
        {
            _movieClips.Add(movieClip);
        }

        public long GetEofOffset()
        {
            return _eofOffset;
        }

        public long GetEofTexOffset()
        {
            return _eofTexOffset;
        }
        public long GetSofTagsOffset()
        {
            return _sofTagsOffset;
        }

        public List<ScObject> GetExports()
        {
            return _exports;
        }

        public string GetInfoFileName()
        {
            return _infoFile;
        }

        public string GetTextureFileName()
        {
            return _textureFile;
        }

        public List<ScObject> GetMovieClips()
        {
            return _movieClips;
        }

        public List<Matrix> GetMatrixs()
        {
            return _matrixs;
        }

        public void addMatrix(Matrix matrix)
        {
            _matrixs.Add(matrix);
        }
        public List<ScObject> GetShapes()
        {
            return _shapes;
        }
        public List<ScObject> GetPendingChanges()
        {
            return _pendingChanges;
        }
        public ushort getShapesChunksCount()
        {
            ushort total = 0;
            foreach (ScObject sco in _shapes)
            {
                total = (ushort) (total + sco.Children.Count);
            }

            return total;
        }

        public long GetStartExportsOffset()
        {
            return _exportStartOffset;
        }

        public List<ScObject> GetTextures()
        {
            return _textures;
        }

        public void SetEofOffset(long offset)
        {
            _eofOffset = offset;
        }


        public void SetEofTexOffset(long offset)
        {
            _eofTexOffset = offset;
        }

        public void SetSofTagsOffset(long offset)
        {
            _sofTagsOffset = offset;
        }

        public void SetStartExportsOffset(long offset)
        {
            _exportStartOffset = offset;
        }

        public void Save(FileStream input, FileStream texinput)
        {
            int exportAdd = 0;
            int textureAdd = 0;
            int movieClipAdd = 0;
            int shapeAdd = 0;
            int shapeChunkAdd = 0;
            int matrixAdd = 0;

            // Flushing depending edits.
            List<ScObject> exports = new List<ScObject>();
            foreach (ScObject data in _pendingChanges)
            {
                switch (data.GetDataType())
                {
                    case 7: // Exports
                        exports.Add(data);
                        exportAdd += 1;
                        break;

                    case 2: // Texture
                        data.Write(texinput);
                        textureAdd += 1;
                        break;

                    case 1: // MovieClip
                        data.Write(input);
                        movieClipAdd += 1;
                        goto case -256;

                    case 0: // Shape
                        data.Write(input);
                        shapeAdd += 1;
                        goto case -256;

                    case 99: // ShapeChunk
                        data.Write(input);
                        shapeChunkAdd += 1;
                        break;

                    case -256:
                        if (data.GetDataType() == -256)
                            throw new Exception("Datatype equals -256 Save()");

                        this.SetEofOffset(input.Position);
                        input.Write(new byte[] { 0, 0, 0, 0, 0 });
                        break;

                    default:
                        data.Write(input);
                        break;
                }
            }
            _pendingChanges.Clear();

            if (exports.Count > 0)
            {
                foreach (ScObject data in exports)
                {
                    data.Write(input);
                }
            }

            if (_pendingMatrixs.Count > 0)
            {
                input.Seek(0, SeekOrigin.Begin);

                using (MemoryStream newData = new MemoryStream())
                {
                    byte[] tillMatrixEndData = new byte[_eofMatrixOffset];
                    input.Read(tillMatrixEndData, 0, (int)_eofMatrixOffset);

                    newData.Write(tillMatrixEndData, 0, tillMatrixEndData.Length);

                    foreach (Matrix matrix in _pendingMatrixs)
                    {
                        newData.WriteByte(8);
                        newData.Write(BitConverter.GetBytes(24), 0, 4);

                        Matrix newMatrix = new Matrix(matrix.Elements[0] / 0.00097656f, matrix.Elements[1] / 0.00097656f, matrix.Elements[2] / 0.00097656f,
                            matrix.Elements[3] / 0.00097656f, matrix.Elements[4] * 20f, matrix.Elements[5] * 20f);
                        
                        for (int i = 0; i < 6; i++)
                        {
                            newData.Write(BitConverter.GetBytes((int) newMatrix.Elements[i]), 0, 4);
                        }

                        _eofMatrixOffset = newData.Position;

                        matrixAdd += 1;
                    }

                    tillMatrixEndData = null;

                    byte[] restData = new byte[input.Length - _eofMatrixOffset];
                    input.Read(tillMatrixEndData, (int)_eofMatrixOffset, (int) restData.Length);

                    newData.Write(restData, 0, restData.Length);

                    input.Seek(0, SeekOrigin.Begin);
                    newData.Seek(0, SeekOrigin.Begin);
                    newData.CopyTo(input);
                }
            }

            // Saving metadata/header.
            input.Seek(0, SeekOrigin.Begin);
            input.Write(BitConverter.GetBytes((ushort)this._shapeCount + shapeAdd), 0, 2);
            input.Write(BitConverter.GetBytes((ushort)this._movieClipCount + movieClipAdd), 0, 2);
            input.Write(BitConverter.GetBytes((ushort)this._textures.Count), 0, 2); // Add
            input.Read(new byte[2], 0, 2); // SKIP
            input.Write(BitConverter.GetBytes((ushort)this._matrixCount + matrixAdd), 0, 2);

            Console.WriteLine($"SaveSC: Done saving Exports: {exportAdd} | MovieClips: {movieClipAdd} | Shapes: {shapeAdd} | Shape Chunks: {shapeChunkAdd} | Textures: {textureAdd} | Matrixs {matrixAdd}");
        }

        public void Load()
        {
            var sw = Stopwatch.StartNew();
            while (true)
            {
                using (var texReader = new BinaryReader(File.OpenRead(_textureFile)))
                {
                    Byte[] IsCompressed = texReader.ReadBytes(2);
                    if (BitConverter.ToString(IsCompressed) == "53-43")
                    {
                        DialogResult result = MessageBox.Show("The tool detected that you have load a compressed file.\nWould you like to decompress and load it?", @"SC File is Compressed", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);

                        if (result == DialogResult.Yes)
                        {

                            byte[] version = texReader.ReadBytes(4);
                            byte[] hashLength = texReader.ReadBytes(4);
                            var hash = texReader.ReadBytes(hashLength[3]);

                            if (version[3] == 1 || version[3] == 3 || version[3] == 0)
                            {
                                string scCompressionType = BitConverter.ToString(texReader.ReadBytes(4));
                                texReader.Close();

                                if (scCompressionType == "53-43-4C-5A")
                                {
                                    Console.WriteLine("LZHAM Compression for " + _textureFile);
                                    Lzma.Decompress(_textureFile);
                                }
                                else if (scCompressionType == "28-B5-2F-FD")
                                {
                                    Console.WriteLine("Zstandard Compression for " + _textureFile);
                                    zstandard.decompress(_textureFile);
                                }
                                else
                                {
                                    Console.WriteLine("LZMA Compression for " + _textureFile);
                                    Lzma.Decompress(_textureFile);
                                }
                            }

                            continue;
                        }
                        break;
                    }

                    Console.WriteLine(texReader.BaseStream.Length);
                    texReader.BaseStream.Seek(0, SeekOrigin.Begin);
                    while (true)
                    {
                        long texoffset = texReader.BaseStream.Position;
                        var packetId = texReader.ReadByte();
                        var packetSize = texReader.ReadUInt32();
                        if (packetSize > 0)
                        {
                            var tex = new Texture(this);
                            tex.SetOffset(texoffset);
                            tex.Read(packetId, packetSize, texReader);

                            if (texoffset + packetSize + 5 != texReader.BaseStream.Position)
                                throw new Exception("Reading less or more bytes for texture");

                            this._textures.Add(tex);
                            if (texReader.BaseStream.Position != texReader.BaseStream.Length)
                            {
                                continue;
                            }
                        }
                        _eofTexOffset = texoffset;
                        break;
                    }
                }
                break;
            }

            while (true)
            {
                using (var reader = new BinaryReader(File.OpenRead(_infoFile)))
                {
                    Byte[] IsCompressed = reader.ReadBytes(2);
                    Console.WriteLine(BitConverter.ToString(IsCompressed) == "53-43");
                    if (BitConverter.ToString(IsCompressed) == "53-43")
                    {
                        DialogResult result = MessageBox.Show("The tool detected that you have load a compressed file.\nWould you like to decompress and load it?", @"SC File is Compressed", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);

                        if (result == DialogResult.Yes)
                        {

                            byte[] version = reader.ReadBytes(4);
                            byte[] hashLength = reader.ReadBytes(4);
                            var hash = reader.ReadBytes(hashLength[3]);

                            if (version[3] == 1 || version[3] == 3 || version[3] == 0)
                            {
                                string scCompressionType = BitConverter.ToString(reader.ReadBytes(4));
                                reader.Close();

                                if (scCompressionType == "53-43-4C-5A")
                                {
                                    Console.WriteLine("LZHAM Compression for " + _infoFile);
                                    Lzma.Decompress(_textureFile);
                                }
                                else if (scCompressionType == "28-B5-2F-FD")
                                {
                                    Console.WriteLine("Zstandard Compression for " + _infoFile);
                                    zstandard.decompress(_infoFile);
                                }
                                else
                                {
                                    Console.WriteLine("LZMA Compression for " + _infoFile);
                                    Lzma.Decompress(_infoFile);
                                }
                            }

                            continue;
                        }
                        break;
                    }

                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    _shapeCount = reader.ReadUInt16();
                    _movieClipCount = reader.ReadUInt16();
                    _textureCount = reader.ReadUInt16();
                    _textFieldCount = reader.ReadUInt16();
                    _matrixCount = reader.ReadUInt16();
                    var colorTransformCount = reader.ReadUInt16();

                    // 5 useless bytes
                    reader.ReadByte();
                    reader.ReadUInt16();
                    reader.ReadUInt16();

                    _exportStartOffset = reader.BaseStream.Position;
                    _exportCount = reader.ReadUInt16();

#if DEBUG
                    Console.WriteLine(@"ExportCount: " + _exportCount);
                    Console.WriteLine(@"ShapeCount: " + _shapeCount);
                    Console.WriteLine(@"MovieClipCount: " + _movieClipCount);
                    Console.WriteLine(@"TextureCount: " + _textureCount);
                    Console.WriteLine(@"TextFieldCount: " + _textFieldCount);
                    Console.WriteLine(@"Matrix2x3Count: " + _matrixCount);
                    Console.WriteLine(@"ColorTransformCount: " + colorTransformCount);
#endif
                    
                    // Reads the Export IDs.
                    for (int i = 0; i < _exportCount; i++)
                    {
                        var export = new Export(this);
                        export.SetId(reader.ReadUInt16());
                        _exports.Add(export);
                    }

                    // Reads the Export names.
                    for (int i = 0; i < _exportCount; i++)
                    {
                        var nameLength = reader.ReadByte();
                        if (nameLength != 255)
                        {
                            var name = Encoding.UTF8.GetString(reader.ReadBytes(nameLength));
                            var export = (Export)_exports[i];
                            export.SetExportName(name);
                        }
                    }

                    int shapeIndex = 0;
                    int movieClipIndex = 0;
                    int movieClipModifierIndex = 0;
                    int textFieldIndex = 0;
                    int matrixIndex = 0;

#pragma warning disable CS0219 // CHECK

                    bool canLoadTex = true;
                    bool _texDependency = false;

                    _sofTagsOffset = reader.BaseStream.Position;

                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        long offset = reader.BaseStream.Position;

                        var datatag = reader.ReadByte();
                        var tagSize = reader.ReadUInt32();

                        var tag = datatag.ToString("X2");

                        if (tagSize < 0)
                            throw new Exception("Negative tag length. Tag " + tag);

                        switch (tag)
                        {
                            case "00": //0
                                if (_shapeCount != shapeIndex ||
                                _movieClipCount != movieClipIndex)
                                {
                                    throw new Exception("Didn't load whole .sc properly.");
                                }

                                _eofOffset = offset;
                                foreach (ScObject t in _exports)
                                {
                                    int index = _movieClips.FindIndex(movie => movie.Id == t.Id);
                                    if (index != -1)
                                        ((Export)t).SetDataObject((MovieClip)_movieClips[index]);
                                }
                                break;

                            case "18": //24 - not using check
                                if (tagSize > 6)
                                {
                                    var texture = new Texture(this);
                                    texture.SetOffset(offset);
                                    texture.Read(datatag, tagSize, reader);
                                    //_eofTexOffset = reader.BaseStream.Position;
                                    this._textures.Add(texture);
                                }
                                else
                                {
                                    var pixelFormat = reader.ReadByte();
                                    var width = reader.ReadUInt16();
                                    var height = reader.ReadUInt16();

                                    Console.WriteLine("pixelFormat: " + pixelFormat);
                                    Console.WriteLine("width: " + width);
                                    Console.WriteLine("height: " + height);
                                }
                                break;

                            case "12": //18
                                if (shapeIndex >= _shapeCount)
                                    throw new Exception($"Trying to load too many shapes.\n Index: {shapeIndex} | Count: {_shapeCount}");

                                var shape = new Shape(this);
                                shape.SetOffset(offset);
                                shape.Read(reader, tag);
                                this._shapes.Add(shape);

                                shapeIndex += 1;
                                break;

                            case "23":
                                goto case "0C";

                            case "0C": //12 = 0C | NEW 35 = 23
                                if (movieClipIndex >= _movieClipCount)
                                    throw new Exception($"Trying to load too many movieclips.\n Index: {movieClipIndex} | Count: {_movieClipCount}");

                                var movieClip = new MovieClip(this, datatag);
                                movieClip.SetOffset(offset);
                                ushort clipId = movieClip.ReadMV(reader, tag);
                                _movieClips.Add(movieClip);

                                movieClipIndex += 1;
                                break;

                            case "08": //8 Matrix
                                if (matrixIndex >= _matrixCount)
                                    throw new Exception($"Trying to load too many shapes.\n Index: {shapeIndex} | Count: {_shapeCount}");

                                float[] Points = new float[6];
                                for (int Index = 0; Index < 6; Index++)
                                {
                                    Points[Index] = reader.ReadInt32();
                                }
                                Matrix _Matrix = new Matrix(Points[0] * 0.00097656f, Points[1] * 0.00097656f, Points[2] * 0.00097656f,
                                    Points[3] * 0.00097656f, Points[4] / 20f, Points[5] / 20f);
                                this._matrixs.Add(_Matrix);

                                _eofMatrixOffset = reader.BaseStream.Position;

                                matrixIndex++;
                                break;

                            case "09": //9 Colors Transform
                                var ra = reader.ReadByte();
                                var ga = reader.ReadByte();
                                var ba = reader.ReadByte();
                                var am = reader.ReadByte();
                                var rm = reader.ReadByte();
                                var gm = reader.ReadByte();
                                var bm = reader.ReadByte();
                                this._colors.Add(new Tuple<Color, byte, Color>(Color.FromArgb(ra, ga, ba), am, Color.FromArgb(rm, gm, bm)));
                                break;

                            case "0D": // 13
                                reader.ReadInt32();
                                throw new Exception("TAG_TIMELINE_INDEXES no longer in use");

                            case "17": // 23
                                // TODO
                                break;

                            case "1E": // 30
                                // CUSTOM TEXFILE
                                break;

                            case "2C": // 30
                                if (textFieldIndex >= _textFieldCount)
                                    throw new Exception("Trying to load too many TextFields from ");

                                reader.ReadBytes(Convert.ToInt32(tagSize)); // TODO

                                textFieldIndex += 1;
                                break;

                            case "20": // 32
                                var stg1Count = reader.ReadByte();
                                if (stg1Count != 255)
                                {
                                    reader.Read(new byte[stg1Count], 0, stg1Count);
                                }

                                var stg2Count = reader.ReadByte();
                                if (stg2Count != 255)
                                {
                                    reader.Read(new byte[stg2Count], 0, stg2Count);
                                }
                                //TODO
                                break;

                            case "25": //37
                                ushort movieClipModifierCount = reader.ReadUInt16();

                                for (int i = 0; i < movieClipModifierCount; i++)
                                {
                                    this._movieClipsModifier.Add(new MovieClipModifierOriginal(this));
                                }
                                break;

                            case "1A": //26
                                canLoadTex = false;
                                _texDependency = true;
                                break;

                            case "28": //40
                                MovieClipModifierOriginal mcmo = new MovieClipModifierOriginal(this);
                                mcmo.Read(reader, tag);

                                this._movieClipsModifier[movieClipModifierIndex++] = mcmo;
                                break;

                            default:
                                Console.WriteLine($"LoadingSC: Unknown Tag {datatag} with size {tagSize}");
                                reader.ReadBytes(Convert.ToInt32(tagSize));
                                break;
                        }
                    }

                    if (_movieClips.Count < _movieClipCount)
                        Console.WriteLine($"Loaded less MovieClips than expected.\nSCCount: {_movieClipCount} | LoadCount: {_movieClips.Count}, IndexCount {movieClipIndex}");

                    if (_shapes.Count < _shapeCount)
                        Console.WriteLine($"Loaded less shapes than expected.\nSCCount: {_shapeCount} | LoadCount: {_shapes.Count}, IndexCount {shapeIndex}");

                    sw.Stop();
                    Program.Interface.Text = $@"SC Editor :  {Path.GetFileNameWithoutExtension(_textureFile)}";
                    Program.Interface.Update();
                    Console.WriteLine(@"SC File loading finished in {0}ms", sw.Elapsed.TotalMilliseconds);
                }
                break;
            }
        }

        public int exportExists(string exportName)
        {
            int value = this._exports.FindIndex(exp => exp.GetName() == exportName);
            return value;
        }

        public int shapeExists(int id)
        {
            int value = this._shapes.FindIndex(shp => shp.Id == id);
            return value;
        }
    }
}
