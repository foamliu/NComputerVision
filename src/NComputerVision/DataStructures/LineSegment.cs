

namespace NComputerVision.DataStructures
{
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// foamliu, 2009/02/11, 线段.
    /// 
    /// </summary>
    public class LineSegment
    {
        private List<Point> m_pair;

        public List<Point> Pair
        {
            get { return m_pair; }
        }

        public LineSegment()
        {
            m_pair = new List<Point>();
        }

    }
}
