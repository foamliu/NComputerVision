using System;
using System.Collections.Generic;
using System.Text;

namespace NComputerVision.Contour
{
    public class LevelSetContourPoint
    {
        private int m_x;
        private int m_y;
        private int m_grayScale;

        public int X
        {
            get { return m_x; }
        }

        public int Y
        {
            get { return m_y; }
        }

        public int GrayScale
        {
            get { return m_grayScale; }
        }

        public LevelSetContourPoint(int x, int y, int grayScale)
        {
            m_x = x;
            m_y = y;
            m_grayScale = grayScale;
        }

    }

    // foamliu, 2009/02/09, 取最大值.
    //
    public class GrayScaleComparer : IComparer<LevelSetContourPoint>
    {
        public int Compare(LevelSetContourPoint pt1, LevelSetContourPoint pt2)
        {
            return System.Math.Sign(pt1.GrayScale - pt2.GrayScale);
        }

    }
}
