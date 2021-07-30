using System.Collections.Generic;
using System.Linq;

namespace HexagonalWpf
{
    internal class ClusterPack
    {
        public string Id { get; set; }
        public string CurrentId { get; set; }
        private readonly List<Cluster> _clusters;

        public ClusterPack(string path)
        {
            Id = path;
            _clusters = new List<Cluster>();
        }

        public void Add(Cluster c)
        {
            _clusters.Add(c);
            CurrentId = c.ClusterId;
        }

        private Cluster CloneCluster(int id)
        {
            var prev = new Cluster(_clusters.OrderBy(x => x.ClusterId).ToList()[id].ClusterId);
            foreach (var e in _clusters.OrderBy(x => x.ClusterId).ToList()[id].GetList)
            {
                prev.Add(e);
            }

            return prev;
        }

        public void Renumbering()
        {
            if (_clusters.Count < 2) return;

            for (var i = 1; i < _clusters.Count; i++)
            {
                var prev = CloneCluster(i - 1);

                foreach (var elem in _clusters[i].GetList)
                {
                    //elem.Id = prev.Count > 0 ? prev.GetNearerId(elem.Element) : _clusters[i].GenerateNextId();
                    elem.Id = prev.Count > 0 ? prev.GetRelativeNearerId(elem.GetRelativeCenter(_clusters[i].CenterPosition)) : _clusters[i].GenerateNextId();
                    prev.RemoveById(elem.Id);
                }
            }
        }

        public Cluster GetById(string id)
        {
            return _clusters.FirstOrDefault(x => x.ClusterId == id);
        }


        public Cluster PrevById(string id)
        {
            var index = _clusters.FindIndex(x => x.ClusterId == id);
            if (index > 0) index--;
            CurrentId = _clusters[index].ClusterId;
            return _clusters[index];
        }

        public Cluster NextById(string id)
        {
            var index = _clusters.FindIndex(x => x.ClusterId == id);
            if (index < _clusters.Count - 1) index++;
            CurrentId = _clusters[index].ClusterId;
            return _clusters[index];
        }
    }
}
