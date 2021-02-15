using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ZedGraph;

namespace TestEMGU1
{
    internal class Chains
    {
        private readonly List<PointListItem> m_OriginsList;
        private readonly List<Graph> GraphChains;
        private readonly PointListItem StartPoint;

        public Chains(List<PointListItem> originsList, PointListItem fPoint)
        {
            m_OriginsList = originsList;
            StartPoint = fPoint;
            GraphChains = new List<Graph>();
            FindChains(StartPoint);
        }

        private bool IsHaveNeighbors(int ItemId)
        {
           
            var self = m_OriginsList.FirstOrDefault(x => x.Id() == ItemId);
            if (self == null) return false;
            {
                var nbs = m_OriginsList.OrderBy(x => x.GetDistance(self.GetPoint())).Take(4).ToList();
                foreach (var itm in nbs)
                {
                    if (self.IsTouched(itm)) return true;
                }
            }
            return false;
        }
        
        private void FindChains(PointListItem startItem)
        {
            while (m_OriginsList.Count>0)
            {
                
                if (IsHaveNeighbors(startItem.Id()))
                {
                    //var ch = new Chain(startItem);
                }
                else m_OriginsList.Remove(startItem);
            }
        }
    }

    internal class Chain
    {
        private readonly PointListItem m_Self;
        private readonly List<PointListItem> m_LinkItems;
        private readonly List<PointListItem> m_OrigItems;

        public Chain(PointListItem self, IEnumerable<PointListItem> orig)
        {
            this.m_Self = self;
            m_LinkItems = new List<PointListItem>();
            m_OrigItems = new List<PointListItem>();
            foreach (var itm in orig) m_OrigItems.Add(itm);
        }

        public IEnumerable<PointListItem> Generate()
        {
            m_OrigItems.Remove(m_Self);
            AddLink(m_Self);
            GenerateRecurse(m_Self);
            return m_LinkItems;
        }

        private void GenerateRecurse(PointListItem current)
        {
            
            var nbh = FindTouchedList(current);
            foreach (var itm in nbh)
            {
                var tmp = m_OrigItems.FirstOrDefault(x => x.Id() == itm);
                m_OrigItems.Remove(tmp);
                AddLink(tmp);
                GenerateRecurse(tmp);
            }
            
        }
        
        private IEnumerable<int> FindTouchedList(PointListItem self)
        {
            var res = new List<int>();
            
            if (self == null) return res;

            var nbs = m_OrigItems.OrderBy(x => x.GetDistance(self.GetPoint())).Take(4).ToList();
            foreach (var itm in nbs)
            {
                if (self.IsTouched(itm)) res.Add(itm.Id());
            }

            return res;
        }

        private bool AddLink(PointListItem item)
        {
            if (m_LinkItems.Any(x => x.Id() == item.Id()))
                return false;
            m_LinkItems.Add(item);
            return true;
        }

        public AngleCollection GetAngles()
        {
            var angles = new AngleCollection();
            for (var i=1; i<=m_LinkItems.Count-2; i++) {
                angles.Add(new AngleItem(m_LinkItems[i-1], m_LinkItems[i], m_LinkItems[i+1]));
            }

            return angles;
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

    class ChainLink
    {

    }
}
