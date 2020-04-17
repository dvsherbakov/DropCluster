using System;
using System.Drawing;

namespace TestEMGU1
{
    public readonly struct PointItem : IEquatable<PointItem>
    {
        public int Id { get; }

        public PointF Point { get; }

        public float Radius { get; }

        public PointItem(int id, PointF point, float radius)
        {
            Id = id;
            Point = point;
            Radius = radius;
        }

        public bool Equals(PointItem other)
        {
            return Id == other.Id && Point == other.Point && Math.Abs(Radius - other.Radius) < 10e-5;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equals((PointItem) obj);
        }

        public override int GetHashCode() => Id;
    }
}