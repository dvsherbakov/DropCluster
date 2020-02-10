using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEMGU1
{
    class AngleCollection
    {
        public List<AngleItem> AnglesList { get; }

        public AngleCollection()
        {
            AnglesList = new List<AngleItem>();
        }

        public void Add(AngleItem item)
        {
            var p = AnglesList.Where(x=>(x.GetId(0)==item.GetId(0)) && (x.GetId(1) == item.GetId(1)) && (x.GetId(2)==item.GetId(2)) 
            || (x.GetId(0) == item.GetId(2)) && (x.GetId(1) == item.GetId(1)) && (x.GetId(2) == item.GetId(0)));
            if ((p.Count()==0)
                && (item.GetId(0) != item.GetId(1))
                && (item.GetId(0) != item.GetId(2))
                && (item.GetId(1) != item.GetId(2))
                )
                AnglesList.Add(item);
        }

        public List<AngleItem> GetGollection()
        {
            return AnglesList;
        }
    }
}
