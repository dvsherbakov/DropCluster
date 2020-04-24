using System.Collections.Generic;
using System.Linq;

namespace TestEMGU1
{
    class AngleCollection
    {
        private readonly List<AngleItem> angles;

        public AngleCollection()
        {
            angles = new List<AngleItem>();
        }

        public void Add(AngleItem item)
        {
            var p = angles.Where(x =>
                x.GetId(0) == item.GetId(0) && x.GetId(1) == item.GetId(1) && x.GetId(2) == item.GetId(2)
                || x.GetId(0) == item.GetId(2) && x.GetId(1) == item.GetId(1) && x.GetId(2) == item.GetId(0));
            if (!HasAngleItem(item, p)) angles.Add(item);
        }

        public AngleItem[] GetAngles() => angles.ToArray();

        private bool HasAngleItem(AngleItem item, IEnumerable<AngleItem> p)
        {
            return p.Any() || item.GetId(0) == item.GetId(1) || item.GetId(0) == item.GetId(2) ||
                   item.GetId(1) == item.GetId(2);
        }
    }
}