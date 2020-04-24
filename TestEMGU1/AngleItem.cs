using System;

namespace TestEMGU1
{
    internal class AngleItem : IEquatable<AngleItem>
    {
        private readonly PointItem a;
        private readonly PointItem b;
        private readonly PointItem c;

        public AngleItem(PointItem a, PointItem b, PointItem c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public Tuple<int, int, int> GetIds()
        {
            return new Tuple<int, int, int>(a.Id, b.Id, c.Id);
        }

        public int GetId(int id)
        {
            switch (id)
            {
                case 0:
                    return a.Id;
                case 1:
                    return b.Id;
                default:
                    return c.Id;
            }
        }

        public Tuple<double, double> Angle_point()
        {
            double x1 = a.Point.X - b.Point.X, x2 = c.Point.X - b.Point.X;
            double y1 = a.Point.Y - b.Point.Y, y2 = c.Point.Y - b.Point.Y;
            var d1 = CalculateDiameter(x1, y1);
            var d2 = CalculateDiameter(x2, y2);
            var anl = Math.Acos((x1 * x2 + y1 * y2) / (d1 * d2)) * 180 / Math.PI;
            return Tuple.Create(anl, (x1 * x2 + y1 * y2) / (d1 * d2));
        }


        public int GetAngleDirection()
        {
            var first = a.Point.Y - b.Point.Y;
            var second = -(a.Point.X - b.Point.X);
            var third = -(first * a.Point.X + second * a.Point.Y);
            var res = first * c.Point.X + second * c.Point.Y + third;
            if (Math.Abs(res) < 0.000001) return 0;
            if (res > 0) return 1;
            if (res < 0) return -1;
            return 0;
        }

        public bool Equals(AngleItem other)
        {
            if (other == null) return false;

            return a.Equals(other.a) && b.Equals(other.b) && c.Equals(other.c);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equals(obj as AngleItem);
        }

        public override int GetHashCode()
        {
            return (((a.Id << 2) ^ b.Id) << 2) ^ c.Id;
        }

        private double CalculateDiameter(double x, double y) => Math.Sqrt(x * x + y * y);
    }
}