using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEMGU1
{
    class AngleItem
    {
        PointListItem A;
        PointListItem B;
        PointListItem C;
        
        public AngleItem(PointListItem a, PointListItem b, PointListItem c)
        {
            A = a;
            B = b;
            C = c;
        }

        public Tuple<int, int, int> GetIds()
        {
            return new Tuple<int, int, int>(A.Id(), B.Id(), C.Id());
        }

        public int GetId(int i)
        {
            switch (i)
            {
                case 0:
                    return A.Id();
                case 1:
                    return B.Id();
                default:
                    return C.Id();
            }
        }

        public Tuple<double, double> Angle_point()
        {
            double x1 = A.GetPoint().X - B.GetPoint().X, x2 = C.GetPoint().X - B.GetPoint().X;
            double y1 = A.GetPoint().Y - B.GetPoint().Y, y2 = C.GetPoint().Y - B.GetPoint().Y;
            var d1 = Math.Sqrt(x1 * x1 + y1 * y1);
            var d2 = Math.Sqrt(x2 * x2 + y2 * y2);
            var anl = Math.Acos((x1 * x2 + y1 * y2) / (d1 * d2)) * 180 / Math.PI;
            return Tuple.Create(anl, (x1 * x2 + y1 * y2) / (d1 * d2));
        }

        public int GetAngleDirection()
        {
            var a = A.GetPoint().Y - B.GetPoint().Y;
            var b = -(A.GetPoint().X - B.GetPoint().X);
            var c = -(a * A.GetPoint().X + b * A.GetPoint().Y);
            var res = (a * C.GetPoint().X + b * C.GetPoint().Y + c);
            if (Math.Abs(res) < 0.000001) return 0;
            if (res > 0) return 1;
            if (res < 0) return -1;
            return 0;
        }
    }
}
