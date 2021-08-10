namespace SCEditor.Helpers
{
    public class Rect
    {
        public float startX;
        public float startY;
        public float endX;
        public float endY;

        public Rect(float startX, float startY, float endX, float endY)
        {
            this.startX = startX;
            this.startY = startY;
            this.endX = endX;
            this.endY = endY;
        }

        public float GetWidth()
        {
            return this.endX - this.startX;
        }

        public float GetHeight()
        {
            return this.endY - this.startY;
        }

        public float GetMin(int axis)
        {
            return axis == 0 ? this.startX : this.startY;
        }

        public float GetMax(int axis)
        {
            return axis == 0 ? this.endX : this.endY;
        }
    }
}