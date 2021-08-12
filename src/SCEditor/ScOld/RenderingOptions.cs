namespace SCEditor.ScOld
{
    public class RenderingOptions
    {
        public bool ViewPolygons { get; set; }
        public double zoomScaleFactor { get; set; }

        public RenderingOptions()
        {
            ViewPolygons = false;
            zoomScaleFactor = 0;
        }
    }
}