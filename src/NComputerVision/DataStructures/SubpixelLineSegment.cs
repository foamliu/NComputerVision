

namespace NComputerVision.DataStructures
{

    /// <summary>
    /// foamiu, 2009/02/17, 亚像素线段. 
    /// 
    /// </summary>
    public class SubpixelLineSegment
    {
        private DoublePoint m_start;
        private DoublePoint m_end;

        public DoublePoint Start
        {
            get { return m_start; }
        }

        public DoublePoint End
        {
            get { return m_end; }
        }

        public SubpixelLineSegment(DoublePoint start, DoublePoint end)
        {
            m_start = start;
            m_end = end;
        }

        public SubpixelLineSegment(float startx, float starty, float endx, float endy)
        {
            m_start = new DoublePoint(startx, starty);
            m_end = new DoublePoint(endx, endy); ;
        }


    }
}
