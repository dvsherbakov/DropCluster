using System.Drawing;

namespace TestEMGU1
{
    public class PointInArea
    {
        public static bool IsPointInside(PointF[] polygon, PointF point)
        {
            if (polygon.Length <= 1)
                return false;

            var intersectionsNum = 0;
            var prev = polygon.Length - 1;
            var prevUnder = polygon[prev].Y < point.Y;

            for (var i = 0; i<polygon.Length; ++i)
            {
                var curUnder = polygon[i].Y < point.Y;

                var a = new PointF(polygon[prev].X - point.X, polygon[prev].Y - point.Y);
                var b = new PointF(polygon[i].X - point.X, polygon[i].Y - point.Y);

                var t = (a.X * (b.Y - a.Y) - a.Y * (b.X - a.X));
                if (curUnder && !prevUnder)
                {
                    if (t > 0)
                        intersectionsNum += 1;
                }
                if (!curUnder && prevUnder)
                {
                    if (t< 0)
                        intersectionsNum += 1;
                }

                prev = i;        
                prevUnder = curUnder;        
            }

            return (intersectionsNum&1) != 0;
        }
    }
}