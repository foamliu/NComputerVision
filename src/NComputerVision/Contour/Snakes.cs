

namespace NComputerVision.Contour
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using NComputerVision.Common;
    using NComputerVision.GraphicsLib;
    using NComputerVision.DataStructures;

    /// <summary>
    /// foamliu, 2009/02/17, 可变形轮廓.
    /// 
    /// </summary>
    public class Snakes
    {
        // 图片
        private int[][] m_mat;
        private int m_width;
        private int m_height;

        // foamliu, 2009/02/17, 轮廓曲线由 n 个点表示.
        //
        private SubpixelContour m_curve;
        // foamliu, 2009/02/17, 弹性系数, 倾向于将轮廓收紧.
        private double m_alpha;
        // foamliu, 2009/02/17, 曲率系数, 倾向于类圆轮廓.
        private double m_beta;
        // foamliu, 2009/02/18, 相邻两点的平均距离, 每次迭代更新, 防止控制点集结成簇.
        private double m_averageD;

        public double Alpha
        {
            get { return m_alpha; }
            set { m_alpha = value; }
        }

        public double Beta
        {
            get { return m_beta; }
            set { m_beta = value; }
        }

        public SubpixelContour Contour
        {
            get { return m_curve; }
            set { m_curve = value; }
        }

        /// <summary>
        /// foamliu, 2009/02/17, snaxel的数目.
        /// 
        /// </summary>
        public int Number
        {
            get { return m_curve.Count; }
        }

        public Snakes(int[][] mat)
        {
            m_mat = mat;
            m_width = mat.Length;
            m_height = mat[0].Length;
            m_curve = new SubpixelContour();
            m_averageD = 0.0;
        }

        /// <summary>
        /// foamliu, 2009/02/17, 总的能量.
        /// 
        /// </summary>
        /// <returns></returns>
        //private double Energy()
        //{
        //    return E_int() + E_ext();
        //}

        /// <summary>
        /// foamliu, 2009/02/17, 为每个控制点计算总能量.
        /// 
        /// </summary>
        /// <returns></returns>
        private double Energy(int index)
        {
            //return E_int(index) + E_ext(index);

            return Energy(index, (float)m_curve[index].X, (float)m_curve[index].Y);
        }

        /// <summary>
        /// foamliu, 2009/02/17, 为每个控制点计算总能量.
        /// 
        /// </summary>
        /// <returns></returns>
        private double Energy(int index, float x, float y)
        {
            return E_total(index, x, y);
            //return E_total(index, x, y) + E_total(GetValidIndex(index - 1), x, y) + E_total(GetValidIndex(index + 1), x, y);

            //double e_int = E_int(index, x, y);
            //double e_ext = E_ext(x, y);
            //return e_int + e_ext;
        }

        private double E_total(int index, float x, float y)
        {
            double e_int = E_int(index, x, y);
            double e_ext = E_ext(x, y);
            return e_int + e_ext;
        }

        /// <summary>
        /// foamliu, 2009/02/17, 内能量.
        /// 
        /// </summary>
        /// <returns></returns>
        //private double E_int()
        //{
        //    double elasticity = 0.0;    // 弹性
        //    double stiffness = 0.0;     // 曲率

        //    for (int i = 0; i < m_curve.Count; i++)
        //    {
        //        SubpixelPoint vi = m_curve[i];
        //        SubpixelPoint vi_next = m_curve[GetValidIndex(i + 1)];
        //        SubpixelPoint vi_prev = m_curve[GetValidIndex(i - 1)];

        //        elasticity += Util.Square(vi_next.X - vi.X) + Util.Square(vi_next.Y - vi.Y)-m_averageD;
        //        stiffness += Util.Square(vi_next.X - 2 * vi.X + vi_prev.X) + Util.Square(vi_next.Y - 2 * vi.Y + vi_prev.Y);
        //    }

        //    return m_alpha * elasticity + m_beta * stiffness;
        //}

        /// <summary>
        /// foamliu, 2009/02/17, 为每个控制点计算内能量, 提出有利于减少循环次数.
        /// 
        /// </summary>
        /// <returns></returns>
        private double E_int(int i)
        {
            //double elasticity = 0.0;    // 弹性
            //double stiffness = 0.0;     // 曲率

            //SubpixelPoint vi = m_curve[i];
            //SubpixelPoint vi_next = m_curve[GetValidIndex(i + 1)];
            //SubpixelPoint vi_prev = m_curve[GetValidIndex(i - 1)];

            //elasticity += Util.Square(vi_next.X - vi.X) + Util.Square(vi_next.Y - vi.Y);
            //stiffness += Util.Square(vi_next.X - 2 * vi.X + vi_prev.X) + Util.Square(vi_next.Y - 2 * vi.Y + vi_prev.Y);

            //return m_alpha * elasticity + m_beta * stiffness;

            return E_int(i, (float)m_curve[i].X, (float)m_curve[i].Y);
        }

        /// <summary>
        /// foamliu, 2009/02/17, 为每个控制点计算内能量, 提出有利于减少循环次数.
        /// 
        /// </summary>
        /// <returns></returns>
        private double E_int(int i, float x, float y)
        {
            double elasticity = 0.0;    // 弹性
            double stiffness = 0.0;     // 曲率

            DoublePoint vi_next = m_curve[GetValidIndex(i + 1)];
            DoublePoint vi_prev = m_curve[GetValidIndex(i - 1)];

            //elasticity += Math.Abs(Util.Square(vi_prev.X - x) + Util.Square(vi_prev.Y - y) - m_averageD * m_averageD);
            elasticity += Math.Abs(Util.Square(vi_next.X - x) + Util.Square(vi_next.Y - y) - m_averageD * m_averageD);
            stiffness += Util.Square(vi_next.X - 2 * x + vi_prev.X) + Util.Square(vi_next.Y - 2 * y + vi_prev.Y);

            elasticity *= m_alpha;
            stiffness *= m_beta;

            //return m_alpha * elasticity + m_beta * stiffness;
            return elasticity + stiffness;

            // foamliu, 2009/02/19, 增强版本.

            //double elasticity = 0.0;    // 弹性
            //double stiffness = 0.0;     // 曲率

            //SubpixelPoint vi_next = m_curve[GetValidIndex(i + 1)];
            //SubpixelPoint vi_prev = m_curve[GetValidIndex(i - 1)];

            //elasticity += Math.Abs(Util.Square(vi_prev.X - x) + Util.Square(vi_prev.Y - y)) - m_averageD * m_averageD;

            //float x01 = x - vi_prev.X;
            //float x12 = vi_next.X - x;
            //float y01 = y - vi_prev.Y;
            //float y12 = vi_next.Y - y;
            //double norm01 = x01 * x01 + y01 * y01;
            //double norm12 = x12 * x12 + y12 * y12;
            //double cross = x01 * x12 + y01 * y12;

            //if (Math.Abs(norm01) < float.Epsilon || Math.Abs(norm12) < float.Epsilon)
            //{
            //    stiffness += 1;
            //}
            //else
            //{
            //    stiffness += 1 - cross * cross / norm01 * norm12;
            //}

            //elasticity *= m_alpha;
            //stiffness *= m_beta;

            ////return m_alpha * elasticity + m_beta * stiffness;
            //return elasticity + stiffness;
        }

        /// <summary>
        /// foamliu, 2009/02/17, 外能量, 描述曲线与图像吻合的程度.
        /// 
        /// </summary>
        /// <returns></returns>
        //private double E_ext()
        //{
        //    double gradient = 0.0;
        //    for (int i = 0; i < m_curve.Count; i++)
        //    {
        //        SubpixelPoint vi = m_curve[i];
        //        int vx = Convert.ToInt32(vi.X);
        //        int vy = Convert.ToInt32(vi.Y);

        //        double gx = Util.Gradient_Prewitt_X(m_mat, vx, vy);
        //        double gy = Util.Gradient_Prewitt_Y(m_mat, vx, vy);
        //        gradient += gx * gx + gy * gy;
        //    }
        //    return -gradient;
        //}

        /// <summary>
        /// foamliu, 2009/02/17, 为每个控制点计算外能量, 提出有利于减少循环次数.
        /// 
        /// </summary>
        /// <returns></returns>
        private double E_ext(int i)
        {
            double gradient = 0.0;
            DoublePoint vi = m_curve[i];
            int vx = Convert.ToInt32(vi.X);
            int vy = Convert.ToInt32(vi.Y);

            double gx = Util.Gradient_Prewitt_X(m_mat, vx, vy);
            double gy = Util.Gradient_Prewitt_Y(m_mat, vx, vy);
            gradient += gx * gx + gy * gy;
            return -gradient;
        }

        /// <summary>
        /// foamliu, 2009/02/17, 为每个控制点计算外能量, 提出有利于减少循环次数.
        /// 
        /// </summary>
        /// <returns></returns>
        private double E_ext(float x, float y)
        {
            double gradient = 0.0;
            int dwX = Convert.ToInt32(x);
            int dwY = Convert.ToInt32(y);
            double gx = Util.Gradient_Prewitt_X(m_mat, dwX, dwY);
            double gy = Util.Gradient_Prewitt_Y(m_mat, dwX, dwY);
            gradient += gx * gx + gy * gy;
            return -gradient;
        }


        /// <summary>
        /// foamliu, 2009/02/17, 初始化轮廓为一个圆.
        /// 
        /// </summary>
        public void InitializeCircle(int xc, int yc, int radius, int number)
        {
            for (int i = 0; i < number; i++)
            {
                double theta = 2 * Math.PI * i / number;
                int xi = Convert.ToInt32(xc + radius * Math.Cos(theta));
                int yi = Convert.ToInt32(yc - radius * Math.Sin(theta));
                m_curve.Add(new DoublePoint(xi, yi));
            }
        }

        /// <summary>
        /// foamliu, 2009/02/18, 初始化轮廓为一个矩形.
        /// 
        /// </summary>
        public void InitializeRectangle(int x, int y, int width, int height, int number)
        {
            int circumference = 2 * (width + height);
            int step = Convert.ToInt32(1.0 * circumference / number);

            for (int i = 0; i < width; i += step)
            {
                m_curve.Add(new DoublePoint(x + i, y));
            }
            for (int j = 0; j < height; j += step)
            {
                m_curve.Add(new DoublePoint(x + width, y + j));
            }
            for (int i = width; i > 0; i -= step)
            {
                m_curve.Add(new DoublePoint(x + i, y + height));
            }
            for (int j = height; j > 0; j -= step)
            {
                m_curve.Add(new DoublePoint(x, y + j));
            }
        }

        public void InitializeRectangle(Rectangle rc, int number)
        {
            InitializeRectangle(rc.X, rc.Y, rc.Width, rc.Height, number);
        }

        public void InitializeRectangle(int number)
        {
            Rectangle rc = new Rectangle(0, 0, m_width, m_height);
            InitializeRectangle(rc, number);
        }

        /// <summary>
        /// foamliu, 2009/02/17, 进行迭代.
        /// 这里用贪心算法, 每次在临近的搜索窗口中找使能量最小的点.
        /// 
        /// 注意:
        ///     1. 不保证收敛.
        ///     2. 需要仔细选择初始轮廓.
        /// 
        /// </summary>
        public void GreedyOptimize()
        {
            int numChanged;

            do
            {
                numChanged = 0;

                for (int i = 0; i < this.Number; i++)
                {
                    OptimizeInSearchWindow(i, ref numChanged);
                }

                UpdateAverageDistance();
            }
            while (numChanged > 0); // 当在上次迭代中没有变化则停止迭代
        }

        /// <summary>
        /// foamliu, 2009/02/17, 更新平均距离.
        /// 
        /// </summary>
        private void UpdateAverageDistance()
        {
            double perimeter = 0.0;
            DoublePoint vi_prev;
            DoublePoint vi;

            for (int i = 0; i < this.Number; i++)
            {
                vi_prev = m_curve[GetValidIndex(i - 1)];
                vi = m_curve[i];
                perimeter += Math.Sqrt(Util.Square(vi.X - vi_prev.X) + Util.Square(vi.Y - vi_prev.Y));
            }

            m_averageD = perimeter / this.Number;
        }

        /// <summary>
        /// foamliu, 2009/02/17, 在 5 × 5 搜索窗口中对本控制点进行优化.
        /// 
        /// </summary>
        /// <param name="i"></param>
        private void OptimizeInSearchWindow(int index, ref int numChanged)
        {
            DoublePoint vi = m_curve[index];
            int vx = Convert.ToInt32(vi.X);
            int vy = Convert.ToInt32(vi.Y);
            int xmin = vx;
            int ymin = vy;
            double minE = Energy(index);

            for (int i = 0; i < Util.SearchWindow.Length; i++)
            {
                int x = vx + Util.SearchWindow[i][0];
                int y = vy + Util.SearchWindow[i][1];
                if (Util.InRect(x, y, m_width, m_height))
                {
                    double energy = Energy(index, x, y);
                    if (minE > energy)
                    {
                        minE = energy;
                        xmin = x;
                        ymin = y;
                    }
                }
            }

            if (xmin != vi.X || ymin != vi.Y)
            {
                numChanged++;
                vi.X = xmin;
                vi.Y = ymin;
            }
        }

        /// <summary>
        /// foamliu, 2009/02/19, 进行迭代.
        /// 这里使用动态规划.
        /// 
        /// </summary>
        public void DPOptimize()
        {
        }

        /// <summary>
        /// foamliu, 2009/02/19, 进行迭代.
        /// 这里使用梯度下降.
        /// 
        /// </summary>
        public void GradientDescentOptimize()
        {
        }

        private int GetValidIndex(int index)
        {
            int valid = index;
            if (valid < 0)
            {
                valid += m_curve.Count;
            }
            else if (valid >= m_curve.Count)
            {
                valid -= m_curve.Count;
            }
            return valid;
        }
    }
}
