using System;
using System.Collections.Generic;
using NComputerVision.Common;
using NComputerVision.DataStructures;
using NComputerVision.GraphicsLib;

namespace NComputerVision.Contour
{
    public class Hough_FitCircle_2
    {
        const double DegreePerRadian = 180 / System.Math.PI;
        const double RadianPerDegree = System.Math.PI / 180;
        const int ThetaStep = 5;                // 每步改变量

        const double ThetaStepRadian = ThetaStep / DegreePerRadian;   // 弧度计算

        const int RadiusStep = 1; // 每步改变量            
        const double Gradient_Threshold = 10;   // 梯度阈值
        const double Value_Threshold = 64;   // 累加器阈值
        const double GrayValue_Threshold = 10;   // 累加器阈值
        const double Rejection_Threshold = 20;   // 拒绝阈值: 当新找到的圆跟之前的圆太相近

        const int NumberOfCircles_Threshold = 4;

        private int[][] m_mat;
        private int m_width, m_height;
        private int m_maxRadius;
        private int m_radiusLen;

        // 累加数组
        private double[, ,] m_ihist;



        public Hough_FitCircle_2(int[][] mat)
        {
            this.m_mat = mat;

            this.m_width = m_mat.Length;
            this.m_height = m_mat[0].Length;

            //m_maxRadius = System.Convert.ToInt32(Math.Sqrt(m_width * m_width + m_height * m_height)) / 2;
            // foamliu, 2009/02/16, 进一步限制一下半径.
            m_maxRadius = 80;

            m_radiusLen = (int)(m_maxRadius / RadiusStep) + 1;

            // 累加数组
            //this.m_ihist = Util.BuildMat(dLen, thetaLen);
            this.m_ihist = new double[m_width, m_height, m_radiusLen];

        }

        /// <summary>
        /// foamliu, 2009/02/12, 将灰度图像中的直线段加到累加器中.
        /// 
        /// </summary>
        public void AccumulateCircles()
        {
            //double rad;       // 半径
            int radQ;         // 量化半径  

            for (int y = 0; y < m_height; y += 2)
            {
                for (int x = 0; x < m_width; x += 2)
                {
                    //if (m_mat[x][y] < GrayValue_Threshold)
                    //    continue;
                    // foamliu, 2009/02/16, 看梯度不看亮度.


                    // 允许的半径值, 从20开始.
                    //
                    for (radQ = 20; radQ <= 80; radQ++)
                    {
                        int radius = radQ * RadiusStep;

                        // foamliu, 2009/02/16, 充分利用梯度信息可以节省大量计算时间.
                        //
                        double dx = Util.Gradient(m_mat, x, y, ConvKernel.Sobel_Gx); // 水平梯度
                        double dy = Util.Gradient(m_mat, x, y, ConvKernel.Sobel_Gy); // 垂直梯度
                        double dmag = System.Math.Sqrt(dx * dx + dy * dy);               // 梯度的幅度
                        if (dmag > Gradient_Threshold)
                        {
                            double theta = System.Math.Atan2(dy, dx);
                            if (theta < 0)
                                theta = 2 * System.Math.PI + theta;

                            // 圆心
                            int xc = x - Convert.ToInt32(radius * System.Math.Cos(theta));
                            int yc = y - Convert.ToInt32(radius * System.Math.Sin(theta));

                            DoPoint(xc, yc, x, y, radQ);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// foamliu, 2009/02/16, 对圆上的点相应的累加器单元增加
        /// 
        /// </summary>
        /// <param name="xc">圆心</param>
        /// <param name="yc">圆心</param>
        /// <param name="x">圆边</param>
        /// <param name="y">圆边</param>
        /// <param name="radQ">半径量化</param>
        private void DoPoint(int xc, int yc, int x, int y, int radQ)
        {
            if (xc < 0 || xc >= m_width || yc < 0 || yc >= m_height)
                return;
            // 相应的累加器单元增加
            //m_ihist[x, y, radQ] += m_mat[xc][yc];
            //foamliu, 2009/02/16, 需要对小圆进行补偿否则对小圆不公平.
            //
            m_ihist[x, y, radQ] += m_mat[xc][yc] / radQ;
            //foamliu, 2009/02/16, 忽略低灰度点.
            //if (m_mat[xc][yc] > GrayValue_Threshold)
            //    m_ihist[x, y, radQ] += m_mat[xc][yc] / radQ;
        }



        /// <summary>
        /// foamliu, 2009/02/12, 找圆.
        /// 
        /// </summary>
        /// <param name="IHIST"></param>
        public List<Circle> FindCircles()
        {
            List<Circle> circles = new List<Circle>();

            int cx, cy, radQ;
            double v = PickGreatestBin(out cx, out cy, out radQ);
            int numberOfLines = 0;

            // foamliu, 2009/02/16, 因为经常会调整参数, 还是用数目控制更方便.
            while (/*v > Value_Threshold && */numberOfLines < NumberOfCircles_Threshold)
            {
                // foamliu, 2009/02/11, 先实现一个简单版本.

                //circles.Add(new Circle(cx, cy, radQ * RadiusStep));
                // foamliu, 2009/02/12, 以便有所控制.
                //
                if (AddCircle(circles, cx, cy, radQ * RadiusStep))
                {
                    numberOfLines++;
                }

                m_ihist[cx, cy, radQ] = 0;
                v = PickGreatestBin(out cx, out cy, out radQ);
            }

            return circles;
        }

        /// <summary>
        /// foamliu, 2009/02/12, 过于接近的将不接受.       
        /// 
        /// </summary>
        private bool AddCircle(List<Circle> circles, int cx, int cy, int radius)
        {
            foreach (Circle circle in circles)
            {
                if (System.Math.Abs(circle.X - cx) + System.Math.Abs(circle.Y - cy) + System.Math.Abs(circle.Radius - radius) < Rejection_Threshold)
                    return false;
            }

            circles.Add(new Circle(cx, cy, radius));
            return true;
        }

        /// <summary>
        /// foamliu, 2009/02/11, 返回最大累加器的值.
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="thetaQ"></param>
        /// <param name="dQ"></param>
        private double PickGreatestBin(out int cx, out int cy, out int rad)
        {
            double max = double.MinValue;
            cx = 0;
            cy = 0;
            rad = 0;

            for (int y = 0; y < m_height; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    // 允许的半径值
                    for (int radQ = 0; radQ < m_radiusLen; radQ++)
                    {
                        if (max < m_ihist[x, y, radQ])
                        {
                            max = m_ihist[x, y, radQ];
                            cx = x;
                            cy = y;
                            rad = radQ;
                        }
                    }
                }
            }

            return max;
        }
    }
}
