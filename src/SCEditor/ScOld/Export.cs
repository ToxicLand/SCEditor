using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.IO;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using System.Windows.Forms;

namespace SCEditor.ScOld
{
    public class Export : ScObject
    {
        #region Constructors
        public Export(ScFile scFile)
        {
            _scFile = scFile;
        }
        #endregion

        #region Fields & Properties
        private ushort _exportId;
        private string _exportName;
        private MovieClip _dataObject;
        private ScFile _scFile;

        public override ushort Id => _exportId;
        public override List<ScObject> Children => getChildren();
        public override SCObjectType objectType => SCObjectType.Export;
        #endregion

        #region Methods

        public List<ScObject> getChildren()
        {
            if (_dataObject != null)
                return _dataObject.Children;

            return null;
        }
        public override ScObject GetDataObject()
        {
            return _dataObject;
        }

        public override int GetDataType()
        {
            return 7;
        }

        public override string GetDataTypeName()
        {
            return "Exports";
        }

        public override string GetInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/!\\ Experimental Rendering");
            sb.AppendLine("");
            sb.AppendLine("ExportId: " + _exportId);
            if (_dataObject != null)
                sb.AppendLine("MovieClipId: " + _dataObject.Id);
            sb.AppendLine("Polygons: " + Children.Count);
            return sb.ToString();
        }

        public override string GetName()
        {
            return _exportName;
        }

        public override void Rename(string name) => _exportName = name;

        public override void Write(FileStream input)
        {
            input.Seek(0, SeekOrigin.Begin);

            byte[] file = new byte[input.Length];
            input.Read(file, 0, file.Length);

            int cursor = (int)_scFile.GetStartExportsOffset();
            input.Seek(_scFile.GetStartExportsOffset(), SeekOrigin.Begin);

            // Write Export Count
            ushort exportCount = BitConverter.ToUInt16(file, cursor);
            input.Write(BitConverter.GetBytes((ushort)(exportCount + 1)), 0, 2);
            cursor += 2;

            // Write Export ID
            input.Seek(exportCount * 2, SeekOrigin.Current);
            cursor += exportCount * 2;
            input.Write(BitConverter.GetBytes(_exportId), 0, 2);

            // Write OLD Export Names
            for (int i = 0; i < exportCount; i++)
            {
                byte nameLength = file[cursor];
                cursor++;
                byte[] exportName = new byte[nameLength];
                Array.Copy(file, cursor, exportName, 0, nameLength);
                input.WriteByte(nameLength);
                input.Write(exportName, 0, nameLength);
                cursor += nameLength;
            }

            // Write Export Name
            input.WriteByte((byte)_exportName.Length);
            input.Write(Encoding.UTF8.GetBytes(_exportName), 0, (byte)_exportName.Length);

            while (cursor < file.Length)
            {
                input.WriteByte(file[cursor]);
                cursor++;
            }

            _scFile.SetSofTagsOffset(_scFile.GetSofTagsOffset() + cursor);
            _scFile.SetEofOffset(_scFile.GetEofOffset() + 2 + 1 + _exportName.Length);
        }

        public void SetDataObject(MovieClip sd)
        {
            _dataObject = sd;
        }

        public void SetId(ushort id)
        {
            _exportId = id;
        }

        public void SetExportName(string name)
        {
            _exportName = name;
        }
        public override Bitmap Render(RenderingOptions options)
        {
            try
            {
                if (Children.Count > 0)
                {
                    List<PointF> A = new List<PointF>();

                    // FINAL SIZE WITH MATRIX = POINTS
                    for (int shapeIndex = 0; shapeIndex < Children.Count; shapeIndex++)
                    {
                        Shape shapeToRender = (Shape)Children[shapeIndex];
                        Matrix matrixData = new Matrix(1, 0, 0, 1, 0, 0);

                        int matrixIdx = -1;

                        if (options.editedMatrixs.Count > 0)
                        {
                            matrixIdx = options.editedMatrixs.FindIndex(data => data.childrenId == shapeToRender.Id);
                        }    

                        if (matrixIdx != -1)
                        {
                            matrixData = options.editedMatrixs[matrixIdx].matrixData;

                            foreach (ShapeChunk chunk in shapeToRender.GetChunks())
                            {
                                PointF[] newXY = new PointF[chunk.XY.Length];

                                for (int xyIdx = 0; xyIdx < newXY.Length; xyIdx++)
                                {
                                    float xNew = matrixData.Elements[4] + matrixData.Elements[0] * chunk.XY[xyIdx].X + matrixData.Elements[2] * chunk.XY[xyIdx].Y;
                                    float yNew = matrixData.Elements[5] + matrixData.Elements[1] * chunk.XY[xyIdx].X + matrixData.Elements[3] * chunk.XY[xyIdx].Y;

                                    newXY[xyIdx] = new PointF(xNew, yNew);
                                }

                                A.AddRange(newXY);
                            }
                        }
                        else
                        {
                            PointF[] pointsXY = shapeToRender.Children.SelectMany(chunk => ((ShapeChunk)chunk).XY).ToArray();
                            A.AddRange(pointsXY.ToArray());
                        }

                    }

                    using (var xyPath = new GraphicsPath())
                    {
                        xyPath.AddPolygon(A.ToArray());

                        var xyBound = Rectangle.Round(xyPath.GetBounds());

                        var width = xyBound.Width;
                        width = width > 0 ? width : 1;

                        var height = xyBound.Height;
                        height = height > 0 ? height : 1;

                        var x = xyBound.X;
                        var y = xyBound.Y;

                        var finalShape = new Bitmap(width, height);
                        Console.WriteLine($"Rendering export ({_exportName}): W:{finalShape.Width} H:{finalShape.Height}\n");

                        foreach (Shape shape in Children)
                        {
                            Matrix matrixData = new Matrix(1, 0, 0, 1, 0, 0);

                            int matrixIdx = -1;

                            if (options.editedMatrixs.Count > 0)
                            {
                                matrixIdx = options.editedMatrixs.FindIndex(data => data.childrenId == shape.Id);
                                
                                if (matrixIdx != -1)
                                    matrixData = options.editedMatrixs[matrixIdx].matrixData;
                            }

                            foreach (ShapeChunk chunk in shape.Children)
                            {
                                var texture = (Texture)_scFile.GetTextures()[chunk.GetTextureId()];
                                if (texture != null)
                                {
                                    Bitmap bitmap = texture.Bitmap;
                                    using (var gpuv = new GraphicsPath())
                                    {
                                        gpuv.AddPolygon(chunk.UV.ToArray());

                                        var gxyBound = Rectangle.Round(gpuv.GetBounds());

                                        int gpuvWidth = gxyBound.Width;
                                        gpuvWidth = gpuvWidth > 0 ? gpuvWidth : 1;

                                        int gpuvHeight = gxyBound.Height;
                                        gpuvHeight = gpuvHeight > 0 ? gpuvHeight : 1;

                                        var shapeChunk = new Bitmap(gpuvWidth, gpuvHeight);

                                        var chunkX = gxyBound.X;
                                        var chunkY = gxyBound.Y;

                                        using (var g = Graphics.FromImage(shapeChunk))
                                        {
                                            gpuv.Transform(new Matrix(1, 0, 0, 1, -chunkX, -chunkY));
                                            g.SetClip(gpuv);
                                            g.DrawImage(bitmap, -chunkX, -chunkY);
                                        }

                                        GraphicsPath gp = new GraphicsPath();
                                        gp.AddPolygon(new[] { new Point(0, 0), new Point(gpuvWidth, 0), new Point(0, gpuvHeight) });

                                        //Calculate transformation Matrix of UV
                                        //double[,] matrixArrayUV = { { polygonUV[0].X, polygonUV[1].X, polygonUV[2].X }, { polygonUV[0].Y, polygonUV[1].Y, polygonUV[2].Y }, { 1, 1, 1 } };
                                        double[,] matrixArrayUV =
                                        {
                                            {
                                                gpuv.PathPoints[0].X, gpuv.PathPoints[1].X, gpuv.PathPoints[2].X
                                            },
                                            {
                                                gpuv.PathPoints[0].Y, gpuv.PathPoints[1].Y, gpuv.PathPoints[2].Y
                                            },
                                            {
                                                1, 1, 1
                                            }
                                        };

                                        PointF[] newXY = new PointF[chunk.XY.Length];

                                        for (int xyIdx = 0; xyIdx < newXY.Length; xyIdx++)
                                        {
                                            float xNew = matrixData.Elements[4] + matrixData.Elements[0] * chunk.XY[xyIdx].X + matrixData.Elements[2] * chunk.XY[xyIdx].Y;
                                            float yNew = matrixData.Elements[5] + matrixData.Elements[1] * chunk.XY[xyIdx].X + matrixData.Elements[3] * chunk.XY[xyIdx].Y;

                                            newXY[xyIdx] = new PointF(xNew, yNew);
                                        }

                                        double[,] matrixArrayXY =
                                        {
                                            {
                                                newXY[0].X, newXY[1].X, newXY[2].X
                                            },
                                            {
                                                newXY[0].Y, newXY[1].Y, newXY[2].Y
                                            },
                                            {
                                                1, 1, 1
                                            }
                                        };

                                        var matrixUV = Matrix<double>.Build.DenseOfArray(matrixArrayUV);
                                        var matrixXY = Matrix<double>.Build.DenseOfArray(matrixArrayXY);
                                        var inverseMatrixUV = matrixUV.Inverse();
                                        var transformMatrix = matrixXY * inverseMatrixUV;
                                        var m = new Matrix((float)transformMatrix[0, 0], (float)transformMatrix[1, 0], (float)transformMatrix[0, 1], (float)transformMatrix[1, 1], (float)transformMatrix[0, 2], (float)transformMatrix[1, 2]);

                                        //Perform transformations
                                        gp.Transform(m);

                                        using (Graphics g = Graphics.FromImage(finalShape))
                                        {

                                            //Set origin
                                            Matrix originTransform = new Matrix();
                                            originTransform.Translate(-x, -y);
                                            g.Transform = originTransform;

                                            g.DrawImage(shapeChunk, gp.PathPoints, gpuv.GetBounds(), GraphicsUnit.Pixel);

                                            if (options.ViewPolygons)
                                            {
                                                gpuv.Transform(m);
                                                g.DrawPath(new Pen(Color.DeepSkyBlue, 1), gpuv);
                                            }
                                            g.Flush();
                                        }
                                    }
                                }
                            }
                        }

                        return finalShape;
                    }
                }
                return null;
            } 
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error Render() exports");
                return null;
            }
        }
    }
    #endregion
}