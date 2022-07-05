using System.Collections.Generic;
using System.Linq;

namespace HexagonalWpf.utils
{
    internal class Regress
    {
        public IEnumerable<ShearPoint> Points { get; }
        public double A { get; set; }
        public double B { get; set; }

        public Regress(IEnumerable<ShearPoint> points)
        {
            this.Points = points;

            var shearPoints = Points as ShearPoint[] ?? Points.ToArray();
            var avgX = shearPoints.Average(x => x.X);
            var avgY = shearPoints.Average(y => y.Brightness);

            double t1 = 0, t2 = 0;
            var t = 1;

            foreach (var point in shearPoints)
            {
                t1 += +(point.Brightness - avgY) * (point.X - avgX);
                t2 += (t - avgX) * (t - avgX);
                t++;
            }

            B = t1 / t2;
            A = avgY - B * avgX;

        }

        public double Zero => (-B) / A;

    }



    internal class ShearPoint
    {
        public int X { get; set; }
        public int Brightness { get; set; }
    }
}
