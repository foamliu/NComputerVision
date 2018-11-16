

namespace NComputerVision.DataStructures
{
    /// <summary>
    /// foamliu, 2009/02/12, 圆.
    /// 
    /// </summary>
    public class Circle
    {
        // 圆心和半径
        private int m_x, m_y, m_rad;

        public int X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        public int Y
        {
            get { return m_y; }
            set { m_y = value; }
        }

        public int Radius
        {
            get { return m_rad; }
            set { m_rad = value; }
        }

        public Circle(int cx, int cy, int rad)
        {
            this.X = cx; this.Y = cy;
            this.m_rad = rad;
        }

    }
}
