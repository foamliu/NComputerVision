using System;
using System.Collections.Generic;
using System.Drawing;
using NComputerVision.Common;
using NComputerVision.DataStructures;
using NComputerVision.GraphicsLib;

namespace NComputerVision.Contour
{
    /// <summary>
    /// foamliu, 2009/02/11, Hough变换找直线段.
    /// 
    /// </summary>
    public class Hough_FitLine
    {
        public int ThetaStep { get; set; }

        public double ThetaStepRadian { get; set; }

        public double DStep { get; set; }
        public double Gradient_Threshold { get; set; }
        public double Value_Threshold { get; set; }
        public double GrayValue_Threshold { get; set; }

        public int NumberOfLines_Threshold { get; set; }

        private int[][] m_mat;
        private int m_width, m_height;
        private int m_maxD;
        private List<Point>[,] m_ptList;
        //private double[][] m_ihist;
        private double[][] m_ihist;

        public double[][] IHist
        {
            get { return m_ihist; }
        }

        public Hough_FitLine(int[][] mat)
        {
            this.m_mat = mat;

            this.m_width = m_mat.Length;
            this.m_height = m_mat[0].Length;

            m_maxD = System.Convert.ToInt32(System.Math.Sqrt(m_width * m_width + m_height * m_height));

            int thetaLen = (int)(360 / ThetaStep) + 1;
            int dLen = (int)(m_maxD / DStep) + 1;

            // 累加数组
            //this.m_ihist = Util.BuildMat(dLen, thetaLen);
            this.m_ihist = Util.BuildMat(dLen, thetaLen);
            // 对此累加器有贡献的点的集合
            this.m_ptList = new List<Point>[dLen, thetaLen];

            // 参数的默认值
            this.ThetaStep = NcvGlobals.ThetaStep;
            this.ThetaStepRadian = NcvGlobals.ThetaStepRadian;
            this.DStep = NcvGlobals.DStep;
            this.Gradient_Threshold = NcvGlobals.Gradient_Threshold;
            this.Value_Threshold = NcvGlobals.Value_Threshold;
            this.GrayValue_Threshold = NcvGlobals.GrayValue_Threshold;
            this.NumberOfLines_Threshold = NcvGlobals.NumberOfLines_Threshold;
        }

        /// <summary>
        /// foamliu, 2009/02/12, 将灰度图像中的直线段加到累加器中.
        /// 
        /// </summary>
        public void AccumulateLines()
        {
            double theta;   // 角度
            int thetaQ;     // 量化角度
            double d;       // 距离
            int dQ;         // 量化距离  

            for (int y = 0; y < m_height; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    if (m_mat[x][y] < GrayValue_Threshold)
                        continue;

                    // foamliu, 2009/02/11, 先实现一个简单版本 -- Hough 变换找直线.
                    //

                    // thetaQ为允许的细分值
                    for (thetaQ = 0; thetaQ < 360 / ThetaStep; thetaQ++)
                    {
                        // 根据量化的thetaQ计算角度, 为弧度在以下区间: [0, 2PI].
                        theta = thetaQ * ThetaStep * NcvGlobals.RadianPerDegree;
                        // 根据thetaQ计算d
                        d = /*Math.Abs(*/x * System.Math.Cos(theta) - y * System.Math.Sin(theta)/*)*/;
                        // d<0 表示不存在通过x,y角度为这样theta的直线.
                        if (d < 0) continue;
                        // 四舍五入为允许的单元值
                        dQ = Convert.ToInt32(d / DStep);

                        // 相应的累加器单元增加
                        m_ihist[dQ][thetaQ] += m_mat[x][y];
                        if (m_ptList[dQ, thetaQ] == null)
                            m_ptList[dQ, thetaQ] = new List<Point>();
                        m_ptList[dQ, thetaQ].Add(new Point(x, y));
                    }

                }
            }
        }

        /// <summary>
        /// foamliu, 2009/02/11, 找线.
        /// 
        /// </summary>
        /// <param name="IHIST"></param>
        public List<LineSegment> FindLines()
        {
            List<LineSegment> segs = new List<LineSegment>();

            int thetaQ, dQ;
            double v = PickGreatestBin(out thetaQ, out dQ);
            int numberOfLines = 0;

            while (v > Value_Threshold && numberOfLines++ < NumberOfLines_Threshold)
            {
                // foamliu, 2009/02/11, 先实现一个简单版本.

                List<Point> ptList = m_ptList[thetaQ, dQ];
                ptList.Sort(new PointComparer());
                int number = ptList.Count;
                LineSegment seg = new LineSegment();
                seg.Pair.Add(ptList[0]);
                seg.Pair.Add(ptList[number - 1]);
                segs.Add(seg);

                m_ihist[thetaQ][dQ] = 0;
                v = PickGreatestBin(out thetaQ, out dQ);
            }

            return segs;
        }

        /// <summary>
        /// foamliu, 2009/02/11, 返回最大累加器的值.
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="thetaQ"></param>
        /// <param name="dQ"></param>
        private double PickGreatestBin(out int thetaQ, out int dQ)
        {
            int width = m_ihist.Length;
            int height = m_ihist[0].Length;

            double max = double.MinValue;
            thetaQ = 0;
            dQ = 0;

            for (int d = 0; d < height; d++)
            {
                for (int theta = 0; theta < width; theta++)
                {
                    if (max < m_ihist[theta][d])
                    {
                        max = m_ihist[theta][d];
                        thetaQ = theta;
                        dQ = d;
                    }
                }
            }

            return max;
        }
    }
}
