

namespace NComputerVision.DataStructures
{

    /// <summary>
    /// 灰度图像
    /// </summary>
    public class GrayScaleImage : BaseImage
    {
        private int[][] m_grayvalue;

        public int[][] GrayValue
        {
            get
            {
                return m_grayvalue;
            }
        }

        public GrayScaleImage(int[][] mat, int width, int height)
        {
            this.m_width = width;
            this.m_height = height;
            this.m_grayvalue = mat;
        }
    }
}
