

namespace NComputerVision.DataStructures
{
    using System;

    /// <summary>
    /// foamliu, 2009/02/10, 灰度值特征.
    /// 
    /// </summary>
    public class GrayValueFeatures
    {
        // 灰度图像
        private int[][] m_mat;

        // 最小灰度值
        private int m_gmin;
        // 最大灰度值
        private int m_gmax;
        // 灰度平均值
        private int m_gmean;
        // 灰度值的方差
        private double m_variance;
        // 灰度值的标准偏差
        private double m_stdDeviation;
        // 重心, Center of Gravity.
        private int m_centerX;
        private int m_centerY;
        // 面积
        private double m_area;
        // 半长短轴和角度
        private double m_majorAxe;
        private double m_minorAxe;

        // 半长轴跟 x 轴之间的角度, [-PI/2, PI/2]
        private double m_angleOfEllipse;

        public int Gmin
        {
            get { return m_gmin; }
            set { m_gmin = value; }
        }

        public int Gmax
        {
            get { return m_gmax; }
            set { m_gmax = value; }
        }

        public int Gmean
        {
            get { return m_gmean; }
            set { m_gmean = value; }
        }

        public double Variance
        {
            get { return m_variance; }
            set { m_variance = value; }
        }

        public double StdDeviation
        {
            get { return m_stdDeviation; }
            set { m_stdDeviation = value; }
        }

        public int CenterX
        {
            get { return m_centerX; }
            set { m_centerX = value; }
        }

        public int CenterY
        {
            get { return m_centerY; }
            set { m_centerY = value; }
        }

        public double Area
        {
            get { return m_area; }
            set { m_area = value; }
        }

        public double SemiMajorAxe
        {
            get { return m_majorAxe; }
            set { m_majorAxe = value; }
        }

        public double SemiMinorAxe
        {
            get { return m_minorAxe; }
            set { m_minorAxe = value; }
        }

        public double AngleOfEllipse
        {
            get { return m_angleOfEllipse; }
            set { m_angleOfEllipse = value; }
        }

        //public GrayValueFeatures()
        //{
        //}

        public GrayValueFeatures(int[][] mat)
        {
            m_mat = mat;
        }

        /// <summary>
        /// foamliu, 2009/02/10, 计算基本的特性, 包括灰度最大最小值平均值, 方差和标准差.
        /// 
        /// </summary>
        public void CalcBasicFeatures()
        {
            int width = m_mat.Length;
            int height = m_mat[0].Length;

            // 第一遍， 记录最大最小值平均值.

            int min = Int32.MaxValue;
            int max = Int32.MinValue;
            double sum = 0.0;
            double gmean;
            double variance;
            double stdDeviation;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (m_mat[x][y] > max)
                        max = m_mat[x][y];
                    if (m_mat[x][y] < min)
                        min = m_mat[x][y];
                    sum += m_mat[x][y];
                }
            }

            gmean = sum / (width * height);

            // 第二遍, 计算方差和标准差.
            //

            sum = 0.0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sum += (m_mat[x][y] - gmean) * (m_mat[x][y] - gmean);
                }
            }

            variance = sum / (width * height - 1);
            stdDeviation = Math.Sqrt(variance);

            this.Gmin = min;
            this.Gmax = max;
            this.Gmean = System.Convert.ToInt32(gmean);
            this.Variance = variance;
            this.StdDeviation = stdDeviation;
        }

        /// <summary>
        /// foamliu, 2009/02/10, 计算与"距"有关的特性.
        /// 
        /// </summary>
        public void CalcMomentFeatures()
        {
            int width = m_mat.Length;
            int height = m_mat[0].Length;
            double area = 0.0;
            double centerX;
            double centerY;
            double u20, u02, u11;
            double majorAxe, minorAxe, angle;

            // 首先是重心和面积
            double sum10 = 0.0, sum01 = 0.0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sum10 += m_mat[x][y] * x;
                    sum01 += m_mat[x][y] * y;
                    area += m_mat[x][y];
                }
            }

            centerX = System.Convert.ToInt32(sum10 / area);
            centerY = System.Convert.ToInt32(sum01 / area);

            // 计算距并由此得到长短轴和角度
            double sum20 = 0.0, sum02 = 0.0, sum11 = 0.0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sum20 += m_mat[x][y] * (x - centerX) * (x - centerX);
                    sum02 += m_mat[x][y] * (y - centerY) * (y - centerY);
                    sum11 += m_mat[x][y] * (x - centerX) * (y - centerY);
                }
            }

            u20 = sum20 / area;
            u02 = sum02 / area;
            u11 = sum11 / area;

            // 半长轴
            majorAxe = Math.Sqrt(2 * (u20 + u02 + Math.Sqrt((u20 - u02) * (u20 - u02) + 4 * u11 * u11)));
            // 半短轴
            minorAxe = Math.Sqrt(2 * (u20 + u02 - Math.Sqrt((u20 - u02) * (u20 - u02) + 4 * u11 * u11)));
            //angle = -0.5 * Math.Atan(2 * u11 / (u02 - u20));
            angle = -0.5 * Math.Atan2(2 * u11, (u02 - u20));

            if (angle >= 0)
                angle = Math.PI / 2 - angle;
            else
                angle = -Math.PI / 2 - angle;

            this.CenterX = Convert.ToInt32(centerX);
            this.CenterY = Convert.ToInt32(centerY);
            this.Area = area;
            this.SemiMajorAxe = majorAxe;
            this.SemiMinorAxe = minorAxe;
            this.AngleOfEllipse = angle;

        }

        public double[] GetRegionFeature()
        {
            return new double[] { m_majorAxe, m_minorAxe, m_area, m_stdDeviation, };
        }
    }
}
