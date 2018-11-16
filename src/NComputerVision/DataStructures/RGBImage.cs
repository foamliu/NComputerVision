

namespace NComputerVision.DataStructures
{

    /// <summary>
    /// RGB图像
    /// </summary>
    public class RgbImage : BaseImage
    {
        private int[][] m_r;
        private int[][] m_g;
        private int[][] m_b;

        public int[][] R
        {
            get
            {
                return m_r;
            }
        }

        public int[][] G
        {
            get
            {
                return m_g;
            }
        }

        public int[][] B
        {
            get
            {
                return m_b;
            }
        }

        public RgbImage(int[][][] rgb_mat, int width, int height)
        {
            this.m_width = width;
            this.m_height = height;
            this.m_r = rgb_mat[0];
            this.m_g = rgb_mat[1];
            this.m_b = rgb_mat[2];
        }
    }
}
