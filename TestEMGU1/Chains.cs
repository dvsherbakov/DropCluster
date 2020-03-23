using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private List<int> TmpIds;

        public Chains(List<PointListItem> originsList)
        {
            m_OriginsList = originsList;
            TmpIds = originsList.Select(x => x.Id()).ToList();
            FindChains();
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
        
        private void FindChains()
        {
            while (m_OriginsList.Count>0)
            {
                var current = m_OriginsList.FirstOrDefault();
                if (IsHaveNeighbors(current.Id()))
                {
                    var ch = new Chain(current);
                }
                else m_OriginsList.Remove(current);
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

        public void FindNeighbors(int item)
        {

            var lst = FindTouchedList(item);
            if (lst.Count > 0)
            {
                Debug.WriteLine(lst.Count);
            }

        }

        private List<int> FindTouchedList(int ItemId)
        {
            var res = new List<int>();
            var self = m_LinkItems.FirstOrDefault(x => x.Id() == ItemId);
            if (self == null) return res;
            {
                var nbs = m_LinkItems.OrderBy(x => x.GetDistance(self.GetPoint())).Take(4).ToList();
                foreach (var itm in nbs)
                {
                    if (self.IsTouched(itm)) res.Add(itm.Id());
                }
            }
            return res;
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

    class ChainLink
    {

    }
}
