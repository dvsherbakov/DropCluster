using System;
using System.Drawing;

namespace TestEMGU1
{
    public class PointInArea
    {
        //bool intersection(PointF start1, PointF end1, PointF start2, PointF end2)
        //{
        //    PointF dir1 = end1 - start1;
        //    PointF dir2 = end2 - start2;

        //    //считаем уравнения прямых проходящих через отрезки
        //    float a1 = -dir1.y;
        //    float b1 = +dir1.x;
        //    float d1 = -(a1 * start1.x + b1 * start1.y);

        //    float a2 = -dir2.y;
        //    float b2 = +dir2.x;
        //    float d2 = -(a2 * start2.x + b2 * start2.y);

        //    //подставляем концы отрезков, для выяснения в каких полуплоскотях они
        //    float seg1_line2_start = a2 * start1.x + b2 * start1.y + d2;
        //    float seg1_line2_end = a2 * end1.x + b2 * end1.y + d2;

        //    float seg2_line1_start = a1 * start2.x + b1 * start2.y + d1;
        //    float seg2_line1_end = a1 * end2.x + b1 * end2.y + d1;

        //    //если концы одного отрезка имеют один знак, значит он в одной полуплоскости и пересечения нет.
        //    if (seg1_line2_start * seg1_line2_end >= 0 || seg2_line1_start * seg2_line1_end >= 0)
        //        return false;

            
        //    return true;
        //}

        public bool IsPointInside(PointF[] polygon, PointF point)
        {
            if (polygon.Length <= 1)
                return false;

            var intersectionsNum = 0;
            var prev = polygon.Length - 1;
            var prevUnder = polygon[prev].Y < point.Y;

            for (var i = 0; i<polygon.Length; ++i)
            {
                var cur_under = polygon[i].Y < point.Y;

                var a = new PointF(polygon[prev].X - point.X, polygon[prev].Y - point.Y);
                var b = new PointF(polygon[i].X - point.X, polygon[i].Y - point.Y);

                var t = (a.X * (b.Y - a.Y) - a.Y * (b.X - a.X));
                if (cur_under && !prevUnder)
                {
                    if (t > 0)
                        intersectionsNum += 1;
                }
                if (!cur_under && prevUnder)
                {
                    if (t< 0)
                        intersectionsNum += 1;
                }

                prev = i;        
                prevUnder = cur_under;        
            }

            return (intersectionsNum&1) != 0;
        }
    }
}