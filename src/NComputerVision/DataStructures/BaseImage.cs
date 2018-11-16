

namespace NComputerVision.DataStructures
{

    /// <summary>
    /// 图像
    /// </summary>
    public abstract class BaseImage
    {
        protected int m_width;
        protected int m_height;

        public int Width
        {
            get
            {
                return m_width;
            }
        }

        public int Height
        {
            get
            {
                return m_height;
            }
        }
    }
}
