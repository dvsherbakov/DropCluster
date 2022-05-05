using System;
using System.Drawing;

namespace PrepareImageFrm
{
    class ClusterRect
    {
        private PointF f_TopLeft;
        private PointF f_BottomRight;

        public float X1 => f_TopLeft.X;
        public float X2 => f_BottomRight.X;
        public float Y1 => f_TopLeft.Y;
        public float Y2 => f_BottomRight.Y;

        public ClusterRect(PointF p1, PointF p2)
        {
            f_TopLeft = new PointF(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
            f_BottomRight = new PointF(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y));
        }

        public ClusterRect(float x1, float y1, float x2, float y2)
        {
            f_TopLeft = new PointF(Math.Min(x1, x2), Math.Min(y1, y2));
            f_BottomRight = new PointF(Math.Max(x1, x2), Math.Max(y1, y2));
        }
    }
}
