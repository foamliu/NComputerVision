

namespace NComputerVision.DataStructures
{
    using System;
    using System.Drawing;
    using NComputerVision.Common;
    using NComputerVision.GraphicsLib;

    /// <summary>
    /// foamliu, 2009/02/24, 积分图.
    /// 
    /// </summary>
    [Serializable()]
    public class IntegralImage : BaseImage
    {
        // 积分图
        private double[][] m_mat;

        public IntegralImage(int[][] originalImage)
        {
            m_width = originalImage.Length;
            m_height = originalImage[0].Length;
            m_mat = Util.BuildMat(m_width, m_height);

            Calculate(originalImage);
        }


        public IntegralImage(Bitmap bmp)
        {
            int[][] originalImage;
            ImageConvert.Bitmap2Mat(bmp, out originalImage, out m_width, out m_height);

            m_mat = Util.BuildMat(m_width, m_height);

            Calculate(originalImage);
        }

        private void Calculate(int[][] image)
        {
            for (int y = 0; y < m_height; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    m_mat[x][y] = GetValue(x - 1, y) + GetValue(x, y - 1) - GetValue(x - 1, y - 1) + image[x][y];
                }
            }
        }

        public double GetValue(int x, int y)
        {
            if (x == -1 || y == -1)
                return 0;
            return m_mat[x][y];
        }
    }
}
