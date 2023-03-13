using Accord;
using Accord.Imaging;
using Accord.Math.Geometry;
using SCEditor.ScOld;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCEditor.Features
{
    public class blobCounter
    {
        private Blob[] blobs;

        private Dictionary<int, List<IntPoint>> leftEdges = new Dictionary<int, List<IntPoint>>();
        private Dictionary<int, List<IntPoint>> rightEdges = new Dictionary<int, List<IntPoint>>();
        private Dictionary<int, List<IntPoint>> topEdges = new Dictionary<int, List<IntPoint>>();
        private Dictionary<int, List<IntPoint>> bottomEdges = new Dictionary<int, List<IntPoint>>();

        private Dictionary<int, List<IntPoint>> hulls = new Dictionary<int, List<IntPoint>>();
        private Dictionary<int, List<IntPoint>> quadrilaterals = new Dictionary<int, List<IntPoint>>();

        private Bitmap dataBitmap;

        private List<Blob> nonEmptyBlobs;

        private HightlightType highlighting = HightlightType.Quadrilateral;

        public HightlightType GetHightlightType()
        {
            return this.highlighting;
        }

        public void SetHightlightType(HightlightType highlighting)
        {
            this.highlighting = highlighting;
        }
 
        public int getBlobCount(Bitmap bitmapImage)
        {
            dataBitmap = (Bitmap)bitmapImage.Clone();

            for (int column = 0; column < dataBitmap.Height; column++)
            {
                for (int row = 0; row < dataBitmap.Width; row++)
                {
                    Color c = dataBitmap.GetPixel(row, column);

                    if (c.A >= 1)
                    {
                        Color nC = Color.FromArgb(255, 0, 255, 0);
                        dataBitmap.SetPixel(row, column, nC);
                    }
                }
            }

            dataBitmap = Accord.Imaging.Image.Clone(dataBitmap, PixelFormat.Format32bppArgb);

            BlobCounter blobCounter = new BlobCounter();

            blobCounter.ProcessImage(dataBitmap);
            blobCounter.MinHeight = 5;
            blobCounter.MinWidth = 5;
            blobs = blobCounter.GetObjectsInformation();

            GrahamConvexHull grahamScan = new GrahamConvexHull();

            foreach (Blob blob in blobs)
            {
                List<IntPoint> leftEdge = new List<IntPoint>();
                List<IntPoint> rightEdge = new List<IntPoint>();
                List<IntPoint> topEdge = new List<IntPoint>();
                List<IntPoint> bottomEdge = new List<IntPoint>();

                // collect edge points
                blobCounter.GetBlobsLeftAndRightEdges(blob, out leftEdge, out rightEdge);
                blobCounter.GetBlobsTopAndBottomEdges(blob, out topEdge, out bottomEdge);

                leftEdges.Add(blob.ID, leftEdge);
                rightEdges.Add(blob.ID, rightEdge);
                topEdges.Add(blob.ID, topEdge);
                bottomEdges.Add(blob.ID, bottomEdge);

                // find convex hull
                List<IntPoint> edgePoints = new List<IntPoint>();
                edgePoints.AddRange(leftEdge);
                edgePoints.AddRange(rightEdge);

                List<IntPoint> hull = grahamScan.FindHull(edgePoints);
                hulls.Add(blob.ID, hull);

                List<IntPoint> quadrilateral = null;

                // find quadrilateral
                if (hull.Count < 4)
                {
                    quadrilateral = new List<IntPoint>(hull);
                }
                else
                {
                    quadrilateral = PointsCloud.FindQuadrilateralCorners(hull);
                }
                quadrilaterals.Add(blob.ID, quadrilateral);

                // shift all points for vizualization
                IntPoint shift = new IntPoint(1, 1);

                PointsCloud.Shift(leftEdge, shift);
                PointsCloud.Shift(rightEdge, shift);
                PointsCloud.Shift(topEdge, shift);
                PointsCloud.Shift(bottomEdge, shift);
                PointsCloud.Shift(hull, shift);
                PointsCloud.Shift(quadrilateral, shift);
            }

            nonEmptyBlobs = new List<Blob>();
            foreach (Blob b in blobs)
            {
                if (quadrilaterals[b.ID].Count > 1)
                    nonEmptyBlobs.Add(b);
            }

            return nonEmptyBlobs.Count;
        }

        public Bitmap createBlobImage()
        {
            int imageWidth, imageHeight;
            imageWidth = dataBitmap.Width;
            imageHeight = dataBitmap.Height;

            List<PointF> centerBlob = new List<PointF>();
            foreach (Blob b in nonEmptyBlobs)
            {
                centerBlob.Add(new PointF(b.CenterOfGravity.X, b.CenterOfGravity.Y));
            }

            using (GraphicsPath gpuv = new GraphicsPath())
            {
                PointF[] arr = new PointF[4];
                arr[0] = new PointF(0, 0);
                arr[1] = new PointF(imageWidth, 0);
                arr[2] = new PointF(imageWidth, imageHeight);
                arr[3] = new PointF(0, imageHeight);
                gpuv.AddPolygon(arr);

                Graphics g = Graphics.FromImage(dataBitmap);
                Rectangle rect = Rectangle.Round(gpuv.GetBounds());

                Pen borderPen = new Pen(Color.FromArgb(64, 64, 64), 1);
                Pen highlightPen = new Pen(Color.Red);
                Pen highlightPenBold = new Pen(Color.FromArgb(0, 255, 0), 3);
                Pen rectPen = new Pen(Color.Blue);

                if (dataBitmap != null)
                {
                    g.DrawImage(dataBitmap, rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);

                    foreach (Blob blob in blobs)
                    {
                        if (quadrilaterals[blob.ID].Count == 0)
                            continue;

                        Pen pen = highlightPen;

                        highlighting = HightlightType.ConvexHull;

                        g.DrawPolygon(pen, PointsListToArray(quadrilaterals[blob.ID]));
                        g.DrawPolygon(pen, centerBlob.ToArray());
                    }
                }
                else
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(128, 128, 128)),
                        rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
                }
            }

            return dataBitmap;
        }

        public void setBitmap(Bitmap bitmap)
        {
            dataBitmap = bitmap;
        }

        // Convert list of AForge.NET's IntPoint to array of .NET's Point
        private static System.Drawing.Point[] PointsListToArray(List<IntPoint> list)
        {
            System.Drawing.Point[] array = new System.Drawing.Point[list.Count];

            for (int i = 0, n = list.Count; i < n; i++)
            {
                array[i] = new System.Drawing.Point(list[i].X, list[i].Y);
            }

            return array;
        }

        // Draw object's edge
        private static void DrawEdge(Graphics g, Pen pen, List<IntPoint> edge)
        {
            System.Drawing.Point[] points = PointsListToArray(edge);

            if (points.Length > 1)
            {
                g.DrawLines(pen, points);
            }
            else
            {
                g.DrawLine(pen, points[0], points[0]);
            }
        }
    }
}
