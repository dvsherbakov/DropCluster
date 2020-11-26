﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.Structure;

namespace PrepareImageFrm
{
    internal class ClusterElement
    {
        public int Id { get; set; }
        public RotatedRect Element { get; set; }

        public ClusterElement(int id, RotatedRect rect)
        {
            Id = id;
            Element = rect;
        }

        public float Range(RotatedRect el)
        {
            return (float)Math.Sqrt(Math.Pow(Element.Center.X - el.Center.X, 2) + Math.Pow(Element.Center.Y - el.Center.Y, 2));
        }
    }
}
