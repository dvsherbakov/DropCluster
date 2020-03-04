using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace TestEMGU1
{
    internal class Chains
    {
        private readonly List<PointListItem> m_OriginsList;

        public Chains(List<PointListItem> originsList)
        {
            m_OriginsList = originsList;
            FindChains();
        }

        private void FindNeighbors(Chain item)
        {
            m_OriginsList.Remove(item.Self());
            var nbs = m_OriginsList.OrderBy(x => x.GetDistance(item.Self().GetPoint())).Take(4).ToList();
           while (nbs.Any())
           {
               var current = nbs.FirstOrDefault();
               nbs.Remove(current);
               if (!item.Self().IsTouched(current)) 
                if (!item.AddLink(current)) continue;
               m_OriginsList.Remove(current);
               var ch = new Chain(current);
               FindNeighbors(ch);
           }

        }

        private void FindChains()
        {
            while (m_OriginsList.Count>0)
            {
                var current = m_OriginsList.FirstOrDefault();
                //m_OriginsList.Remove(current);
                var ch = new Chain(current);
                FindNeighbors(ch);
            }
        }
    }

    internal class Chain
    {
        private readonly PointListItem m_Self;
        private readonly List<PointListItem> m_LinkItems;

        public Chain(PointListItem self)
        {
            this.m_Self = self;
            m_LinkItems = new List<PointListItem>();
        }

        public bool AddLink(PointListItem item)
        {
            if (m_LinkItems.Select(x => x.Id() == item.Id()).Any()) 
                return false;
            m_LinkItems.Add(item);
            return true;
        }

        public int Id()
        {
            return m_Self.Id();
        }

        public PointListItem Self()
        {
            return m_Self;
        }
    }
}
