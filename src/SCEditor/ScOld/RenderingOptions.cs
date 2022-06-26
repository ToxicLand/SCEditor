using SCEditor.Prompts;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace SCEditor.ScOld
{
    public class RenderingOptions
    {
        public bool ViewPolygons { get; set; }
        public bool LogConsole { get; set; } = true;
        public float polygonLineWidth { get; set; }
        public bool fillPolygon { get; set; }
        public bool InternalRendering { get; set; }
        public Matrix MatrixData { get; set; }
        public double zoomScaleFactor { get; set; }
        public List<OriginalData> editedMatrixs { get; set;  }
        public bool isLooping { get; set; }
        public static bool disableTextFieldRendering { get; set; } = true;
        public Dictionary<ushort, Matrix> editedMatrixPerChildren = new Dictionary<ushort, Matrix>();

        public RenderingOptions()
        {
            ViewPolygons = false;
            fillPolygon = false;
            InternalRendering = false;
            isLooping = true;
            zoomScaleFactor = 0;
            polygonLineWidth = 2;
            MatrixData = null;
            editedMatrixs = new List<OriginalData>();
            editedMatrixPerChildren = new Dictionary<ushort, Matrix>();
        }
    }
}